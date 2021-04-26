using System;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Common.ObjectLocking;
using FunFair.Ethereum.Abi.Encoder.Interfaces;
using FunFair.Ethereum.Contracts.Events;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Events.Interfaces;
using FunFair.Labs.ScalingEthereum.Contracts.GameManager.Events;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.EventHandlers
{
    /// <summary>
    ///     Common base class for all Game round event handlers.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class GameRoundEventHandlerBase<TEvent> : IEventHandler<TEvent>
        where TEvent : EventOutput, IGameRoundEventOutput
    {
        private readonly IObjectLockManager<GameRoundId> _gameRoundLockManager;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="gameRoundDataManager">Game Round Data manager</param>
        /// <param name="gameRoundLockManager">Game Lock manager.</param>
        /// <param name="logger">Logging.</param>
        protected GameRoundEventHandlerBase(IGameRoundDataManager gameRoundDataManager, IObjectLockManager<GameRoundId> gameRoundLockManager, ILogger logger)
        {
            this.GameRoundDataManager = gameRoundDataManager ?? throw new ArgumentNullException(nameof(gameRoundDataManager));
            this._gameRoundLockManager = gameRoundLockManager ?? throw new ArgumentNullException(nameof(gameRoundLockManager));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Game round data manager.
        /// </summary>
        protected IGameRoundDataManager GameRoundDataManager { get; }

        /// <summary>
        ///     Logging.
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc />
        public abstract Task<BlockNumber?> FindSearchFromBlockAsync(NetworkContract contract);

        /// <inheritdoc />
        public abstract int MinimumConfirmationsRequired { get; }

        /// <inheritdoc />
        public async Task<bool> HandleAsync(EventMiningContext eventMiningContext, NetworkEventLog<TEvent> eventOutput)
        {
            this.Logger.LogInformation($"{eventOutput.Network.Name}: Event {this.GetType().Name} Game {eventOutput.Event.GameRoundId}");

            CancellationToken cancellationToken = CancellationToken.None;

            await using (IObjectLock<GameRoundId>? gameRoundLock = await this._gameRoundLockManager.TakeLockAsync(eventOutput.Event.GameRoundId))
            {
                if (gameRoundLock == null)
                {
                    this.Logger.LogWarning(
                        $"{eventOutput.Network.Name}: Event {this.GetType().Name} Game {eventOutput.Event.GameRoundId} - Could not acquire lock, for event in block {eventMiningContext.NetworkBlockHeader.Number.Value} ({eventMiningContext.NetworkBlockHeader.Hash}). Transaction will need retrying... TX: {eventMiningContext.TransactionHash}.");

                    return false;
                }

                GameRound? gameRound = await this.GameRoundDataManager.GetAsync(eventOutput.Event.GameRoundId);

                if (gameRound == null)
                {
                    // Game doesn't exist
                    return true;
                }

                if (gameRound.Status == GameRoundStatus.COMPLETED)
                {
                    // Game already completed - don't need to do anything here.
                    return true;
                }

                bool result = await this.ProcessEventUnderLockAsync(gameRound: gameRound,
                                                                    eventData: eventOutput.Event,
                                                                    transactionHash: eventMiningContext.TransactionHash,
                                                                    networkBlockHeader: eventMiningContext.NetworkBlockHeader,
                                                                    cancellationToken: cancellationToken);

                if (!result)
                {
                    this.Logger.LogWarning(
                        $"{eventOutput.Network.Name}: Event {this.GetType().Name} Game {eventOutput.Event.GameRoundId} - Event Handler Returned False: Incomplete, for event in block {eventMiningContext.NetworkBlockHeader.Number.Value} ({eventMiningContext.NetworkBlockHeader.Hash}). Transaction will need retrying... TX: {eventMiningContext.TransactionHash}.");
                }

                return result;
            }
        }

        /// <summary>
        ///     Process the event.
        /// </summary>
        /// <param name="gameRound">The game round that is being processed.</param>
        /// <param name="eventData">The event data.</param>
        /// <param name="transactionHash">The transaction hash that the transaction was mined in.</param>
        /// <param name="networkBlockHeader">The network block header for the block the transaction was mined in.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>true, if the event was successfully processed; otherwise, false.</returns>
        protected abstract Task<bool> ProcessEventUnderLockAsync(GameRound gameRound,
                                                                 TEvent eventData,
                                                                 TransactionHash transactionHash,
                                                                 INetworkBlockHeader networkBlockHeader,
                                                                 CancellationToken cancellationToken);
    }
}