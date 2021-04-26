using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    ///     Handler for games ending
    /// </summary>
    [SuppressMessage(category: "Microsoft.Naming", checkId: "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Best name for it")]
    [SuppressMessage(category: "ReSharper", checkId: "ClassNeverInstantiated.Global", Justification = "Instantiated by DI")]
    public sealed class EndGameRoundEventHandler : GameRoundEventHandlerBase<EndGameRoundEventOutput>
    {
        private readonly IGameStatisticsPublisher _gameStatisticsPublisher;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="gameRoundDataManager">Game Round Data manager</param>
        /// <param name="gameRoundLockManager">Game Lock manager.</param>
        /// <param name="gameStatisticsPublisher">Game stats publisher.</param>
        /// <param name="logger">Logging.</param>
        public EndGameRoundEventHandler(IGameRoundDataManager gameRoundDataManager,
                                        IObjectLockManager<GameRoundId> gameRoundLockManager,
                                        IGameStatisticsPublisher gameStatisticsPublisher,
                                        ILogger<EndGameRoundEventHandler> logger)
            : base(gameRoundDataManager: gameRoundDataManager, gameRoundLockManager: gameRoundLockManager, logger: logger)
        {
            this._gameStatisticsPublisher = gameStatisticsPublisher ?? throw new ArgumentNullException(nameof(gameStatisticsPublisher));
        }

        /// <inheritdoc />
        public override int MinimumConfirmationsRequired { get; } = Confirmations.MinimumRequired;

        /// <inheritdoc />
        public override Task<BlockNumber?> FindSearchFromBlockAsync(NetworkContract contract)
        {
            return this.GameRoundDataManager.GetEarliestBlockNumberForPendingCloseAsync(contract.Network);
        }

        /// <inheritdoc />
        protected override async Task<bool> ProcessEventUnderLockAsync(GameRound gameRound,
                                                                       EndGameRoundEventOutput eventData,
                                                                       TransactionHash transactionHash,
                                                                       INetworkBlockHeader networkBlockHeader,
                                                                       CancellationToken cancellationToken)
        {
            if (gameRound.Status != GameRoundStatus.COMPLETING)
            {
                // Don't care what status it is in - if its not completing then this event isn't relevant
                return true;
            }

            WinAmount[] winAmounts = eventData
                                     .Players.Zip(second: eventData.WinAmounts, resultSelector: (accountAddress, winAmount) => new WinAmount {AccountAddress = accountAddress, Amount = winAmount})
                                     .ToArray();

            this.Logger.LogInformation($"{networkBlockHeader.Network.Name}: {eventData.GameRoundId}. House Win: {eventData.HouseWinLoss}");

            await this.GameRoundDataManager.SaveEndRoundAsync(gameRoundId: gameRound.GameRoundId,
                                                              blockNumberCreated: networkBlockHeader.Number,
                                                              transactionHash: transactionHash,
                                                              winAmounts: winAmounts,
                                                              houseWinLoss: eventData.HouseWinLoss,
                                                              progressivePotWinLoss: eventData.ProgressivePotWinLoss,
                                                              gameResult: eventData.GameResult,
                                                              history: eventData.History);

            await this._gameStatisticsPublisher.GameRoundEndedAsync(network: networkBlockHeader.Network,
                                                                    gameRoundId: gameRound.GameRoundId,
                                                                    blockNumber: networkBlockHeader.Number,
                                                                    startBlockNumber: gameRound.BlockNumberCreated);

            return true;
        }
    }
}