using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Common.Extensions;
using FunFair.Ethereum.Abi.Encoder.Interfaces;
using FunFair.Ethereum.Client.Interfaces;
using FunFair.Ethereum.Confirmations.Interfaces;
using FunFair.Ethereum.Contracts;
using FunFair.Ethereum.Contracts.Events;
using FunFair.Ethereum.Contracts.Extensions;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Events.Data.Interfaces;
using FunFair.Ethereum.Events.Interfaces;
using FunFair.Ethereum.Events.Services;
using FunFair.Ethereum.Wallet.Interfaces;
using FunFair.Labs.ScalingEthereum.Contracts;
using FunFair.Labs.ScalingEthereum.Contracts.GameManager.Events;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Logic.Games.EventHandlers;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.BackgroundServices.Services
{
    /// <summary>
    ///     Recovery of broken games.
    /// </summary>
    public sealed class BrokenGameRecovery : IBrokenGameRecovery
    {
        private readonly IConfirmationsReadinessChecker _confirmationsReadinessChecker;
        private readonly IContractInfo _contractInfo;
        private readonly IEthereumAccountManager _ethereumAccountManager;
        private readonly IEventDataManager _eventDataManager;
        private readonly IEventDecoder _eventDecoder;
        private readonly IEventHandlerFactory _eventHandlerFactory;
        private readonly IEventSignatureFactory _eventSignatureFactory;
        private readonly IGameManager _gameManager;
        private readonly IGameRoundDataManager _gameRoundDataManager;
        private readonly ILogger<BrokenGameRecovery> _logger;
        private readonly ITransactionLoader _transactionLoader;

        /// <summary>
        ///     Constructor,
        /// </summary>
        /// <param name="gameRoundDataManager">Game Round Data Manager</param>
        /// <param name="gameManager">Game manager</param>
        /// <param name="ethereumAccountManager">Ethereum account manager.</param>
        /// <param name="transactionLoader">Transaction Loader.</param>
        /// <param name="contractInfoRegistry">Contract info registry.</param>
        /// <param name="eventSignatureFactory">Event signature factory.</param>
        /// <param name="eventHandlerFactory">Event Handler Factory.</param>
        /// <param name="eventDataManager">Event data manager.</param>
        /// <param name="eventDecoder">Event Decoder.</param>
        /// <param name="confirmationsReadinessChecker">Confirmations readiness checker.</param>
        /// <param name="logger">Logging.</param>
        public BrokenGameRecovery(IGameRoundDataManager gameRoundDataManager,
                                  IGameManager gameManager,
                                  IEthereumAccountManager ethereumAccountManager,
                                  ITransactionLoader transactionLoader,
                                  IContractInfoRegistry contractInfoRegistry,
                                  IEventSignatureFactory eventSignatureFactory,
                                  IEventHandlerFactory eventHandlerFactory,
                                  IEventDataManager eventDataManager,
                                  IEventDecoder eventDecoder,
                                  IConfirmationsReadinessChecker confirmationsReadinessChecker,
                                  ILogger<BrokenGameRecovery> logger)
        {
            this._gameRoundDataManager = gameRoundDataManager ?? throw new ArgumentNullException(nameof(gameRoundDataManager));
            this._gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            this._ethereumAccountManager = ethereumAccountManager ?? throw new ArgumentNullException(nameof(ethereumAccountManager));
            this._transactionLoader = transactionLoader ?? throw new ArgumentNullException(nameof(transactionLoader));
            this._eventSignatureFactory = eventSignatureFactory ?? throw new ArgumentNullException(nameof(eventSignatureFactory));
            this._eventHandlerFactory = eventHandlerFactory ?? throw new ArgumentNullException(nameof(eventHandlerFactory));
            this._eventDataManager = eventDataManager ?? throw new ArgumentNullException(nameof(eventDataManager));
            this._eventDecoder = eventDecoder ?? throw new ArgumentNullException(nameof(eventDecoder));
            this._confirmationsReadinessChecker = confirmationsReadinessChecker ?? throw new ArgumentNullException(nameof(confirmationsReadinessChecker));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this._contractInfo = contractInfoRegistry.FindContractInfo(WellKnownContracts.GameManager);
        }

        /// <inheritdoc />
        public async Task RecoverAsync(INetworkBlockHeader blockHeader, CancellationToken cancellationToken)
        {
            IReadOnlyList<GameRound> brokenGames = await this._gameRoundDataManager.GetGamesToFixAsync(network: blockHeader.Network, dateTimeOnNetwork: blockHeader.Timestamp);

            this._logger.LogInformation($"{blockHeader.Network.Name}: Found {brokenGames.Count} games that need fixing");

            foreach (var game in brokenGames)
            {
                await this.FixGameAsync(blockHeader: blockHeader, game: game, cancellationToken: cancellationToken);
            }
        }

        private Task FixGameAsync(INetworkBlockHeader blockHeader, GameRound game, CancellationToken cancellationToken)
        {
            switch (game.Status)
            {
                case GameRoundStatus.PENDING: return this.FixPendingGameAsync(blockHeader: blockHeader, game: game, cancellationToken: cancellationToken);

                case GameRoundStatus.COMPLETING: return this.FixCompletingGameAsync(blockHeader: blockHeader, game: game, cancellationToken: cancellationToken);

                default:
                    this._logger.LogWarning($"{blockHeader.Network.Name}: {game.GameRoundId} - in unexpected state: {game.Status.GetName()}");

                    return Task.CompletedTask;
            }
        }

        private async Task FixCompletingGameAsync(INetworkBlockHeader blockHeader, GameRound game, CancellationToken cancellationToken)
        {
            this._logger.LogWarning($"{blockHeader.Network.Name}: {game.GameRoundId} - Needs fixing? to become complete");

            bool handled = await this.AttemptToResolveEventAsync<EndGameRoundEventHandler, EndGameRoundEvent, EndGameRoundEventOutput>(
                blockHeader: blockHeader,
                gameRoundId: game.GameRoundId,
                cancellationToken: cancellationToken);

            if (!handled)
            {
                try
                {
                    INetworkSigningAccount account = this._ethereumAccountManager.GetAccount(new NetworkAccount(network: blockHeader.Network, address: game.CreatedByAccount));

                    await this._gameManager.EndGameAsync(account: account, gameRoundId: game.GameRoundId, networkBlockHeader: blockHeader, cancellationToken: cancellationToken);
                }
                catch
                {
                    await this._gameRoundDataManager.MarkAsBrokenAsync(gameRoundId: game.GameRoundId, closingBlockNumber: blockHeader.Number, exceptionMessage: "Did not complete");
                }
            }
        }

        private async Task FixPendingGameAsync(INetworkBlockHeader blockHeader, GameRound game, CancellationToken cancellationToken)
        {
            this._logger.LogWarning($"{blockHeader.Network.Name}: {game.GameRoundId} - Needs fixing to start");

            bool handled = await this.AttemptToResolveEventAsync<StartGameRoundEventHandler, StartGameRoundEvent, StartGameRoundEventOutput>(
                blockHeader: blockHeader,
                gameRoundId: game.GameRoundId,
                cancellationToken: cancellationToken);

            if (!handled)
            {
                try
                {
                    INetworkSigningAccount account = this._ethereumAccountManager.GetAccount(new NetworkAccount(network: blockHeader.Network, address: game.CreatedByAccount));

                    await this._gameManager.StartGameAsync(account: account, game: game, networkBlockHeader: blockHeader, cancellationToken: cancellationToken);
                }
                catch
                {
                    await this._gameRoundDataManager.MarkAsBrokenAsync(gameRoundId: game.GameRoundId, closingBlockNumber: blockHeader.Number, exceptionMessage: "Did not start");
                }
            }
        }

        private async Task<bool> AttemptToResolveEventAsync<TEventHandler, TEvent, TEventOutput>(INetworkBlockHeader blockHeader, GameRoundId gameRoundId, CancellationToken cancellationToken)
            where TEventHandler : IEventHandler<TEventOutput> where TEvent : Event<TEventOutput>, new() where TEventOutput : EventOutput
        {
            TEvent evt = this._contractInfo.Event<TEvent>();

            EventSignature eventSignature = this._eventSignatureFactory.Create(evt);

            IReadOnlyList<TransactionHash> transactionHashes = await this._gameRoundDataManager.GetTransactionsAsync(gameRoundId: gameRoundId, functionName: evt.Name);

            IReadOnlyList<IPendingNetworkTransaction> transactions =
                await this._transactionLoader.GetTransactionsAsync(network: blockHeader.Network, transactionHashes: transactionHashes, cancellationToken: cancellationToken);
            IReadOnlyList<NetworkTransactionReceipt> receipts =
                await this._transactionLoader.GetTransactionReceiptsAsync(network: blockHeader.Network, transactionHashes: transactionHashes, cancellationToken: cancellationToken);

            bool handled = false;

            foreach (NetworkTransactionReceipt? receipt in receipts)
            {
                IPendingNetworkTransaction transaction = transactions.First(tx => tx.TransactionHash == receipt.TransactionHash);

                IReadOnlyList<TransactionEventLogEntry> logs = receipts.SelectMany(r => r.Logs?.Where(l => l.Topics[0]
                                                                                                            .ToEventSignature() == eventSignature) ?? Array.Empty<TransactionEventLogEntry>())
                                                                       .ToArray();

                IEventDispatcher ed = new EventDispatcher<TEventHandler, TEvent, TEventOutput>(contractInfo: this._contractInfo,
                                                                                               eventSignature: eventSignature,
                                                                                               eventHandlerFactory: this._eventHandlerFactory,
                                                                                               eventDataManager: this._eventDataManager,
                                                                                               eventDecoder: this._eventDecoder,
                                                                                               confirmationsReadinessChecker: this._confirmationsReadinessChecker,
                                                                                               logger: this._logger);

                handled |= await ed.DispatchAsync(network: blockHeader.Network,
                                                  logs: logs,
                                                  networkBlockHeader: blockHeader,
                                                  latestBlockNumberOnNetwork: blockHeader.Number,
                                                  isFresh: false,
                                                  gasUsed: receipt.GasUsed,
                                                  gasPrice: transaction.GasPrice,
                                                  retrievalStrategy: EventRetrievalStrategy.RISKY);
            }

            return handled;
        }
    }
}