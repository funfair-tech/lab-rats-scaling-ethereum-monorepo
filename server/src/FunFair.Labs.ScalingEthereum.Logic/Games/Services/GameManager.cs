using System;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Ethereum.Client.Interfaces.Exceptions;
using FunFair.Ethereum.Crypto.Interfaces;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Transactions.Data.Interfaces.Models;
using FunFair.Ethereum.Wallet.Interfaces;
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
        private readonly IGameRoundDataManager _gameRoundDataManager;
        private readonly IGameStatsPublisher _gameStatsPublisher;
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
        /// <param name="gameStatsPublisher">The game stats publisher</param>
        /// <param name="lowBalanceWatcher">Low balance watcher</param>
        /// <param name="logger">Logger</param>
        public GameManager(IGameRoundDataManager gameRoundDataManager,
                           IRandomSource randomSource,
                           ITransactionService transactionService,
                           IHasher hasher,
                           IGameStatsPublisher gameStatsPublisher,
                           ILowBalanceWatcher lowBalanceWatcher,
                           ILogger<GameManager> logger)
        {
            this._gameRoundDataManager = gameRoundDataManager ?? throw new ArgumentNullException(nameof(gameRoundDataManager));
            this._randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
            this._transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            this._hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._gameStatsPublisher = gameStatsPublisher ?? throw new ArgumentNullException(nameof(gameStatsPublisher));
            this._lowBalanceWatcher = lowBalanceWatcher ?? throw new ArgumentNullException(nameof(lowBalanceWatcher));
        }

        /// <inheritdoc />
        public async Task StartGameAsync(INetworkSigningAccount account, ContractAddress gameContract, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken)
        {
            if (!this._lowBalanceWatcher.DoesAccountHaveEnoughBalance(account))
            {
                this._logger.LogWarning($"{account.Network.Name}: To create new game round there was no enough {account.Network.NativeCurrency} for house address {account.Address}");

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
                                                                 new NetworkContract(network: account.Network, contractAddress: gameContract),
                                                                 seedCommit: seedCommit,
                                                                 seedReveal: seedReveal,
                                                                 roundDuration: GameRoundParameters.RoundDuration,
                                                                 roundTimeoutDuration: GameRoundParameters.RoundTimeoutDuration,
                                                                 blockNumberCreated: networkBlockHeader.Number,
                                                                 transactionHash: pendingTransaction.TransactionHash);

            await this._gameStatsPublisher.GameRoundStartingAsync(network: account.Network, gameRoundId: gameRoundId, transactionHash: pendingTransaction.TransactionHash);
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

                await this._gameStatsPublisher.GameRoundBrokenAsync(network: account.Network, gameRoundId: gameRoundId);

                return;
            }

            this._logger.LogInformation($"{account.Network.Name}: Ending game {gameRoundId}: tx {pendingTransaction.TransactionHash}");

            await this._gameRoundDataManager.BeginCompleteAsync(gameRoundId: gameRoundId, blockNumberCreated: networkBlockHeader.Number, transactionHash: pendingTransaction.TransactionHash);

            await this._gameStatsPublisher.GameRoundEndingAsync(network: account.Network, gameRoundId: gameRoundId, transactionHash: pendingTransaction.TransactionHash, seedReveal: game.SeedReveal);
        }

        /// <inheritdoc />
        public async Task EndGameBettingAsync(INetworkSigningAccount account, GameRoundId gameRoundId, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken)
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
                this._logger.LogError(new EventId(exception.HResult), exception: exception, $"{account.Network.Name}: Failed to end betting for game {gameRoundId}: {exception.Message}");

                await this._gameRoundDataManager.MarkAsBrokenAsync(gameRoundId: gameRoundId, closingBlockNumber: networkBlockHeader.Number, exceptionMessage: exception.Message);

                await this._gameStatsPublisher.GameRoundBrokenAsync(network: account.Network, gameRoundId: gameRoundId);

                return;
            }

            this._logger.LogInformation($"{account.Network.Name}: Ending betting for game {gameRoundId}: tx {pendingTransaction.TransactionHash}");

            await this._gameRoundDataManager.BeginCompleteAsync(gameRoundId: gameRoundId, blockNumberCreated: networkBlockHeader.Number, transactionHash: pendingTransaction.TransactionHash);

            await this._gameStatsPublisher.GameRoundEndingAsync(network: account.Network, gameRoundId: gameRoundId, transactionHash: pendingTransaction.TransactionHash, seedReveal: game.SeedReveal);
        }

        /// <inheritdoc />
        public async Task StartGameAsync(INetworkSigningAccount account, GameRound game, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken)
        {
            PendingTransaction pendingTransaction;

            try
            {
                StartGameRoundInput input = new(roundId: game.GameRoundId, gameAddress: game.GameContract.Address, entropyCommit: game.SeedCommit);

                pendingTransaction = await this._transactionService.SubmitAsync(account: account,
                                                                                transactionContext: new TransactionContext(contextType: @"GAMEROUND", game.GameRoundId.ToString()),
                                                                                input: input,
                                                                                cancellationToken: cancellationToken);
            }
            catch (TransactionWillAlwaysFailException exception)
            {
                this._logger.LogError(new EventId(exception.HResult), exception: exception, $"{networkBlockHeader.Network.Name}: Failed to start game {game.GameRoundId}: {exception.Message}");

                await this._gameRoundDataManager.MarkAsBrokenAsync(gameRoundId: game.GameRoundId, closingBlockNumber: networkBlockHeader.Number, exceptionMessage: exception.Message);

                await this._gameStatsPublisher.GameRoundBrokenAsync(network: account.Network, gameRoundId: game.GameRoundId);

                return;
            }

            this._logger.LogInformation($"{pendingTransaction.Network.Name}: Created game {game.GameRoundId}: tx {pendingTransaction.TransactionHash}");
        }
    }
}