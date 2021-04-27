using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Logic.Games;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Subscription;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Publishers
{
    /// <summary>
    ///     Game stats events publisher
    /// </summary>
    public sealed class GameStatisticsPublisher : PublisherBase, IGameStatisticsPublisher
    {
        private readonly ILogger<GameStatisticsPublisher> _logger;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="authenticatedHubContext">Authenticated hub context.</param>
        /// <param name="publicHubContext">Public hub context.</param>
        /// <param name="groupNameGenerator">Group generator.</param>
        /// <param name="logger">Logging.</param>
        public GameStatisticsPublisher(IHubContext<AuthenticatedHub, IHub> authenticatedHubContext,
                                       IHubContext<PublicHub, IHub> publicHubContext,
                                       IGroupNameGenerator groupNameGenerator,
                                       ILogger<GameStatisticsPublisher> logger)
            : base(authenticatedHubContext: authenticatedHubContext, publicHubContext: publicHubContext, groupNameGenerator: groupNameGenerator)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public Task NoGamesAvailableAsync(EthereumNetwork network)
        {
            this._logger.LogInformation($"{network.Name}: No games available");

            IEnumerable<IHub> hubs = this.GetAllHubs(network: network, includeLocalGroups: false, includeGlobalGroups: true);

            return Task.WhenAll(hubs.Select(hub => hub.NoGamesAvailable()));
        }

        /// <inheritdoc />
        public Task GameRoundStartedAsync(EthereumNetwork network, GameRoundId gameRoundId, int timeLeftInSeconds, BlockNumber blockNumber)
        {
            this._logger.LogInformation($"{network.Name}: Game {gameRoundId} started using . Block number: {blockNumber}. Remaining time: {timeLeftInSeconds}");

            IEnumerable<IHub> hubs = this.GetAllHubs(network: network, includeLocalGroups: false, includeGlobalGroups: true);

            return Task.WhenAll(hubs.Select(hub => hub.GameRoundStarted(roundId: gameRoundId,
                                                                        timeLeftInSeconds: timeLeftInSeconds,
                                                                        blockNumber: blockNumber,
                                                                        (int) GameRoundParameters.InterGameDelay.TotalSeconds)));
        }

        /// <inheritdoc />
        public Task GameRoundStartingAsync(EthereumNetwork network, GameRoundId gameRoundId, TransactionHash transactionHash)
        {
            this._logger.LogInformation($"{network.Name}: Game {gameRoundId} starting. Txn hash {transactionHash}");

            IEnumerable<IHub> hubs = this.GetAllHubs(network: network, includeLocalGroups: false, includeGlobalGroups: true);

            return Task.WhenAll(hubs.Select(hub => hub.GameRoundStarting(roundId: gameRoundId, transactionHash: transactionHash)));
        }

        /// <inheritdoc />
        public Task GameRoundBettingEndingAsync(EthereumNetwork network, GameRoundId gameRoundId, TransactionHash transactionHash)
        {
            this._logger.LogInformation($"{network.Name}: Game {gameRoundId} betting ending. Txn hash {transactionHash}");

            IEnumerable<IHub> hubs = this.GetAllHubs(network: network, includeLocalGroups: false, includeGlobalGroups: true);

            return Task.WhenAll(hubs.Select(hub => hub.BettingEnding(roundId: gameRoundId, transactionHash: transactionHash)));
        }

        /// <inheritdoc />
        public Task GameRoundBettingEndedAsync(EthereumNetwork network, GameRoundId gameRoundId, BlockNumber blockNumber, BlockNumber startBlockNumber)
        {
            this._logger.LogInformation($"{network.Name}: Game {gameRoundId} betting ended. Block number {blockNumber}");

            IEnumerable<IHub> hubs = this.GetAllHubs(network: network, includeLocalGroups: false, includeGlobalGroups: true);

            return Task.WhenAll(hubs.Select(hub => hub.GameRoundEnded(roundId: gameRoundId,
                                                                      blockNumber: blockNumber,
                                                                      (int) GameRoundParameters.InterGameDelay.TotalSeconds,
                                                                      startBlockNumber: startBlockNumber)));
        }

        /// <inheritdoc />
        public Task GameRoundEndingAsync(EthereumNetwork network, GameRoundId gameRoundId, TransactionHash transactionHash, Seed seedReveal)
        {
            this._logger.LogInformation($"{network.Name}: Game {gameRoundId} ending. Txn hash {transactionHash}");

            IEnumerable<IHub> hubs = this.GetAllHubs(network: network, includeLocalGroups: false, includeGlobalGroups: true);

            return Task.WhenAll(hubs.Select(hub => hub.GameRoundEnding(roundId: gameRoundId, transactionHash: transactionHash, seedReveal: seedReveal)));
        }

        /// <inheritdoc />
        public Task GameRoundEndedAsync(EthereumNetwork network, GameRoundId gameRoundId, BlockNumber blockNumber, BlockNumber startBlockNumber)
        {
            this._logger.LogInformation($"{network.Name}: Game {gameRoundId} ended. Block number {blockNumber}");

            IEnumerable<IHub> hubs = this.GetAllHubs(network: network, includeLocalGroups: false, includeGlobalGroups: true);

            return Task.WhenAll(hubs.Select(hub => hub.GameRoundEnded(roundId: gameRoundId,
                                                                      blockNumber: blockNumber,
                                                                      (int) GameRoundParameters.InterGameDelay.TotalSeconds,
                                                                      startBlockNumber: startBlockNumber)));
        }

        public Task GameRoundBrokenAsync(EthereumNetwork network, GameRoundId gameRoundId)
        {
            this._logger.LogInformation($"{network.Name}: Game {gameRoundId} broken.");

            IEnumerable<IHub> hubs = this.GetAllHubs(network: network, includeLocalGroups: false, includeGlobalGroups: true);

            return Task.WhenAll(hubs.Select(hub => hub.GameRoundBroken(roundId: gameRoundId, (int) GameRoundParameters.InterGameDelay.TotalSeconds)));
        }
    }
}