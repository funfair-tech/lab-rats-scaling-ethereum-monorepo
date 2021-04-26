using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Common.ObjectLocking;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Contracts.GameManager.Events;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.EventHandlers
{
    /// <summary>
    ///     Handler for games starting.
    /// </summary>
    [SuppressMessage(category: "Microsoft.Naming", checkId: "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Best name for it")]
    [SuppressMessage(category: "ReSharper", checkId: "ClassNeverInstantiated.Global", Justification = "Instantiated by DI")]
    public sealed class StartGameRoundEventHandler : GameRoundEventHandlerBase<StartGameRoundEventOutput>
    {
        private readonly IGameRoundTimeCalculator _gameRoundTimeCalculator;
        private readonly IGameStatsPublisher _gameStatsPublisher;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="gameStatsPublisher">Game stats web socket publisher</param>
        /// <param name="gameRoundDataManager">Game Round Data manager</param>
        /// <param name="gameRoundLockManager">Game Round Lock manager.</param>
        /// <param name="gameRoundTimeCalculator">Game Round time calculator.</param>
        /// <param name="logger">Logging.</param>
        public StartGameRoundEventHandler(IGameStatsPublisher gameStatsPublisher,
                                          IGameRoundDataManager gameRoundDataManager,
                                          IObjectLockManager<GameRoundId> gameRoundLockManager,
                                          IGameRoundTimeCalculator gameRoundTimeCalculator,
                                          ILogger<StartGameRoundEventHandler> logger)
            : base(gameRoundDataManager: gameRoundDataManager, gameRoundLockManager: gameRoundLockManager, logger: logger)
        {
            this._gameStatsPublisher = gameStatsPublisher ?? throw new ArgumentNullException(nameof(gameStatsPublisher));
            this._gameRoundTimeCalculator = gameRoundTimeCalculator ?? throw new ArgumentNullException(nameof(gameRoundTimeCalculator));
        }

        /// <inheritdoc />
        public override int MinimumConfirmationsRequired { get; } = Confirmations.MinimumRequired;

        /// <inheritdoc />
        public override Task<BlockNumber?> FindSearchFromBlockAsync(NetworkContract contract)
        {
            return this.GameRoundDataManager.GetEarliestBlockNumberForPendingCreationAsync(contract.Network);
        }

        /// <inheritdoc />
        protected override async Task<bool> ProcessEventUnderLockAsync(GameRound gameRound,
                                                                       StartGameRoundEventOutput eventData,
                                                                       TransactionHash transactionHash,
                                                                       INetworkBlockHeader networkBlockHeader,
                                                                       CancellationToken cancellationToken)
        {
            if (gameRound.Status != GameRoundStatus.PENDING)
            {
                // Don't care what status it is in - if its not pending then this event isn't relevant
                return true;
            }

            GameRound newRoundState =
                new(gameRoundId: gameRound.GameRoundId, createdByAccount: gameRound.CreatedByAccount, gameContract: gameRound.GameContract, seedCommit: gameRound.SeedCommit, seedReveal:
                    gameRound.SeedReveal, status: GameRoundStatus.STARTED, roundDuration: gameRound.RoundDuration, roundTimeoutDuration: gameRound.RoundTimeoutDuration, dateCreated:
                    gameRound.DateCreated, dateUpdated: networkBlockHeader.Timestamp, dateStarted: networkBlockHeader.Timestamp, dateClosed: null, blockNumberCreated: networkBlockHeader.Number);

            await this.GameRoundDataManager.ActivateAsync(activationTime: newRoundState.DateStarted!.Value,
                                                          gameRoundId: newRoundState.GameRoundId,
                                                          blockNumberCreated: newRoundState.BlockNumberCreated,
                                                          transactionHash: transactionHash);

            await this._gameStatsPublisher.GameRoundStartedAsync(network: newRoundState.GameContract.Network,
                                                                 gameRoundId: newRoundState.GameRoundId,
                                                                 this._gameRoundTimeCalculator.CalculateTimeLeft(gameRound: newRoundState),
                                                                 blockNumber: newRoundState.BlockNumberCreated);

            return true;
        }
    }
}