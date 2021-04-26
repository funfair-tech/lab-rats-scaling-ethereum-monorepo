using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;
using FunFair.Labs.ScalingEthereum.Logic.Players;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Subscription;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Publishers
{
    /// <summary>
    ///     Publisher or player statistics.
    /// </summary>
    public sealed class PlayerStatisticsPublisherX : PublisherBase, IPlayerStatisticsPublisher
    {
        private readonly ILogger<PlayerStatisticsPublisherX> _logger;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="authenticatedHubContext">Authenticated hub context.</param>
        /// <param name="publicHubContext">Public hub context.</param>
        /// <param name="groupNameGenerator">Group generator.</param>
        /// <param name="logger">Logging.</param>
        public PlayerStatisticsPublisherX(IHubContext<AuthenticatedHub, IHub> authenticatedHubContext,
                                          IHubContext<PublicHub, IHub> publicHubContext,
                                          IGroupNameGenerator groupNameGenerator,
                                          ILogger<PlayerStatisticsPublisherX> logger)
            : base(authenticatedHubContext: authenticatedHubContext, publicHubContext: publicHubContext, groupNameGenerator: groupNameGenerator)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public Task AmountOfPlayersAsync(EthereumNetwork network, int players)
        {
            this._logger.LogInformation($"{network.Name}: Players Online: {players}");

            IEnumerable<IHub> hubs = this.GetAllHubs(network: network, includeLocalGroups: true, includeGlobalGroups: false);

            return Task.WhenAll(hubs.Select(hub => hub.PlayersOnline(players)));
        }
    }
}