using System;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Ethereum.Client.Interfaces.Exceptions;
using FunFair.Ethereum.Contracts;
using FunFair.Ethereum.Crypto.Interfaces;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Transactions.Data.Interfaces.Models;
using FunFair.Ethereum.Wallet.Interfaces;
using FunFair.Labs.ScalingEthereum.Contracts;
using FunFair.Labs.ScalingEthereum.Contracts.GameManager.Transactions;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Logic.Balances;
using FunFair.Random;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.Services
{
    /// <summary>
    ///     Game manager
    /// </summary>
    public sealed class GameManager : IGameManager
    {
        private readonly IContractInfo _gameManager;
        private readonly IGameRoundDataManager _gameRoundDataManager;
        private readonly IGameStatisticsPublisher _gameStatisticsPublisher;
        private readonly IHasher _hasher;
        private readonly ILogger<GameManager> _logger;
        private readonly ILowBalanceWatcher _lowBalanceWatcher;
        private readonly IRandomSource _randomSource;
        private readonly ITransactionService _transactionService;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="gameRoundDataManager">Game round data manager</param>
        /// <param name="randomSource">Random source</param>
        /// <param name="transactionService">Transaction service</param>
        /// <param name="hasher">Hasher</param>
        /// <param name="contractInfoRegistry">Contract info registry.</param>
        /// <param name="gameStatisticsPublisher">The game stats publisher</param>
        /// <param name="lowBalanceWatcher">Low balance watcher</param>
        /// <param name="logger">Logger</param>
        public GameManager(IGameRoundDataManager gameRoundDataManager,
                           IRandomSource randomSource,
                           ITransactionService transactionService,
                           IHasher hasher,
                           IContractInfoRegistry contractInfoRegistry,
                           IGameStatisticsPublisher gameStatisticsPublisher,
                           ILowBalanceWatcher lowBalanceWatcher,
                           ILogger<GameManager> logger)
        {
            this._gameRoundDataManager = gameRoundDataManager ?? throw new ArgumentNullException(nameof(gameRoundDataManager));
            this._randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
            this._transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            this._hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._gameStatisticsPublisher = gameStatisticsPublisher ?? throw new ArgumentNullException(nameof(gameStatisticsPublisher));
            this._lowBalanceWatcher = lowBalanceWatcher ?? throw new ArgumentNullException(nameof(lowBalanceWatcher));

            this._gameManager = contractInfoRegistry.FindContractInfo(WellKnownContracts.GameManager);
        }

        /// <inheritdoc />
        public async Task StartGameAsync(INetworkSigningAccount account, ContractAddress gameContract, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken)
        {
            if (!this._lowBalanceWatcher.DoesAccountHaveEnoughBalance(account))
            {
                this._logger.LogWarning($"{account.Network.Name}: There was no enough {account.Network.NativeCurrency} for house address {account.Address} to create a game");

                return;
            }

            if (!this._gameManager.Addresses.TryGetValue(key: account.Network, out ContractAddress? gameManagerContract))
            {
                return;
            }

            GameRoundId gameRoundId = new(bytes: this._randomSource.Generate(count: GameRoundId.RequiredByteLength));

            // The commit that's public is generated by using the one way hash from the reveal.
            Seed seedReveal = new(bytes: this._randomSource.Generate(count: Seed.RequiredByteLength));
            Seed seedCommit = new(this._hasher.Hash(seedReveal.ToSpan()));

            StartGameRoundInput input = new(roundId: gameRoundId, gameAddress: gameContract, entropyCommit: seedCommit);

            PendingTransaction pendingTransaction = await this._transactionService.SubmitAsync(account: account,
                                                                                               transactionContext: new TransactionContext(contextType: @"GAMEROUND", gameRoundId.ToString()),
                                                                                               input: input,
                                                                                               cancellationToken: cancellationToken);

            this._logger.LogInformation($"{pendingTransaction.Network.Name}: Created game {gameRoundId} tx {pendingTransaction.TransactionHash}");

            await this._gameRoundDataManager.SaveStartRoundAsync(gameRoundId: gameRoundId,
                                                                 createdByAccount: account.Address,
                                                                 network: account.Network,
                                                                 gameContract: gameContract,
                                                                 gameManagerContract: gameManagerContract,
                                                                 seedCommit: seedCommit,
                                                                 seedReveal: seedReveal,
                                                                 roundDuration: GameRoundParameters.RoundDuration,
                                                                 roundTimeoutDuration: GameRoundParameters.RoundTimeoutDuration,
                                                                 blockNumberCreated: networkBlockHeader.Number,
                                                                 transactionHash: pendingTransaction.TransactionHash);

            await this._gameStatisticsPublisher.GameRoundStartingAsync(network: account.Network, gameRoundId: gameRoundId, transactionHash: pendingTransaction.TransactionHash);
        }

        /// <inheritdoc />
        public async Task EndGameAsync(INetworkSigningAccount account, GameRoundId gameRoundId, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken)
        {
            GameRound? game = await this._gameRoundDataManager.GetAsync(gameRoundId);

            if (game == null)
            {
                throw new NotSupportedException();
            }

            PendingTransaction pendingTransaction;

            try
            {
                EndGameRoundInput input = new(roundId: gameRoundId, entropyReveal: game.SeedReveal);

                pendingTransaction = await this._transactionService.SubmitAsync(account: account,
                                                                                transactionContext: new TransactionContext(contextType: @"GAMEROUND", gameRoundId.ToString()),
                                                                                input: input,
                                                                                cancellationToken: cancellationToken);
            }
            catch (TransactionWillAlwaysFailException exception)
            {
                this._logger.LogError(new EventId(exception.HResult), exception: exception, $"{account.Network.Name}: Failed to end game {gameRoundId}: {exception.Message}");

                await this._gameRoundDataManager.MarkAsBrokenAsync(gameRoundId: gameRoundId, closingBlockNumber: networkBlockHeader.Number, exceptionMessage: exception.Message);

                await this._gameStatisticsPublisher.GameRoundBrokenAsync(network: account.Network, gameRoundId: gameRoundId);

                return;
            }

            this._logger.LogInformation($"{account.Network.Name}: Ending game {gameRoundId}: tx {pendingTransaction.TransactionHash}");

            await this._gameRoundDataManager.BeginCompleteAsync(gameRoundId: gameRoundId, blockNumberCreated: networkBlockHeader.Number, transactionHash: pendingTransaction.TransactionHash);

            await this._gameStatisticsPublisher.GameRoundEndingAsync(network: account.Network,
                                                                     gameRoundId: gameRoundId,
                                                                     transactionHash: pendingTransaction.TransactionHash,
                                                                     seedReveal: game.SeedReveal);
        }

        /// <inheritdoc />
        public async Task StopBettingAsync(INetworkSigningAccount account, GameRoundId gameRoundId, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken)
        {
            GameRound? game = await this._gameRoundDataManager.GetAsync(gameRoundId);

            if (game == null)
            {
                throw new NotSupportedException();
            }

            PendingTransaction pendingTransaction;

            try
            {
                StopBettingInput input = new(roundId: gameRoundId);

                pendingTransaction = await this._transactionService.SubmitAsync(account: account,
                                                                                transactionContext: new TransactionContext(contextType: @"GAMEROUND", gameRoundId.ToString()),
                                                                                input: input,
                                                                                cancellationToken: cancellationToken);
            }
            catch (TransactionWillAlwaysFailException exception)
            {
                this._logger.LogError(new EventId(exception.HResult), exception: exception, $"{account.Network.Name}: Failed to stop betting for game {gameRoundId}: {exception.Message}");

#if BROKEN
                await this._gameRoundDataManager.MarkAsBrokenAsync(gameRoundId: gameRoundId, closingBlockNumber: networkBlockHeader.Number, exceptionMessage: exception.Message);
#endif

                await this._gameStatisticsPublisher.GameRoundBrokenAsync(network: account.Network, gameRoundId: gameRoundId);

                return;
            }

            this._logger.LogInformation($"{account.Network.Name}: Stop betting for game {gameRoundId}: tx {pendingTransaction.TransactionHash}");

            await this._gameRoundDataManager.MarkAsBettingClosingAsync(gameRoundId: gameRoundId, blockNumber: networkBlockHeader.Number, transactionHash: pendingTransaction.TransactionHash);

            await this._gameStatisticsPublisher.BettingEndingAsync(network: account.Network, gameRoundId: gameRoundId, transactionHash: pendingTransaction.TransactionHash);
        }

        /// <inheritdoc />
        public async Task StartGameAsync(INetworkSigningAccount account, GameRound game, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken)
        {
            PendingTransaction pendingTransaction;

            try
            {
                StartGameRoundInput input = new(roundId: game.GameRoundId, gameAddress: game.GameContract, entropyCommit: game.SeedCommit);

                pendingTransaction = await this._transactionService.SubmitAsync(account: account,
                                                                                transactionContext: new TransactionContext(contextType: @"GAMEROUND", game.GameRoundId.ToString()),
                                                                                input: input,
                                                                                cancellationToken: cancellationToken);
            }
            catch (TransactionWillAlwaysFailException exception)
            {
                this._logger.LogError(new EventId(exception.HResult), exception: exception, $"{networkBlockHeader.Network.Name}: Failed to start game {game.GameRoundId}: {exception.Message}");

                await this._gameRoundDataManager.MarkAsBrokenAsync(gameRoundId: game.GameRoundId, closingBlockNumber: networkBlockHeader.Number, exceptionMessage: exception.Message);

                await this._gameStatisticsPublisher.GameRoundBrokenAsync(network: account.Network, gameRoundId: game.GameRoundId);

                return;
            }

            this._logger.LogInformation($"{pendingTransaction.Network.Name}: Created game {game.GameRoundId}: tx {pendingTransaction.TransactionHash}");
        }
    }
}