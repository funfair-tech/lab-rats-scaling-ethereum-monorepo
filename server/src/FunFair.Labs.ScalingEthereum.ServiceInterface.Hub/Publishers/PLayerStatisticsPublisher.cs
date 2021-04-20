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
    /// <inheritdoc />
    public sealed class PLayerStatisticsPublisher : IPlayerStatisticsPublisher
    {
        private readonly IHubContext<AuthenticatedHub, IHub> _authenticatedHubContext;

        private readonly IGroupNameGenerator _groupNameGenerator;
        private readonly ILogger<PLayerStatisticsPublisher> _logger;
        private readonly IHubContext<PublicHub, IHub> _publicHubContext;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="authenticatedHubContext">Authenticated hub context.</param>
        /// <param name="publicHubContext">Public hub context.</param>
        /// <param name="groupNameGenerator">Group generator.</param>
        /// <param name="logger">Logging.</param>
        public PLayerStatisticsPublisher(IHubContext<AuthenticatedHub, IHub> authenticatedHubContext,
                                         IHubContext<PublicHub, IHub> publicHubContext,
                                         IGroupNameGenerator groupNameGenerator,
                                         ILogger<PLayerStatisticsPublisher> logger)
        {
            this._authenticatedHubContext = authenticatedHubContext ?? throw new ArgumentNullException(nameof(authenticatedHubContext));
            this._publicHubContext = publicHubContext ?? throw new ArgumentNullException(nameof(publicHubContext));
            this._groupNameGenerator = groupNameGenerator ?? throw new ArgumentNullException(nameof(groupNameGenerator));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public Task AmountOfPlayersAsync(EthereumNetwork network, int players)
        {
            this._logger.LogInformation($"{network.Name}: Players Online: {players}");

            IEnumerable<IHub> hubs = this.GetAllHubs(network: network, includeLocalGroups: true, includeGlobalGroups: false);

            return Task.WhenAll(hubs.Select(hub => hub.PlayersOnline(players)));
        }

        private IEnumerable<IHub> GetAllHubs(EthereumNetwork network, bool includeLocalGroups, bool includeGlobalGroups)
        {
            if (includeLocalGroups)
            {
                yield return this._publicHubContext.Clients.Group(this._groupNameGenerator.GenerateLocal(network));

                yield return this._authenticatedHubContext.Clients.Group(this._groupNameGenerator.GenerateLocal(network));
            }

            if (includeGlobalGroups)
            {
                yield return this._publicHubContext.Clients.Group(this._groupNameGenerator.GenerateGlobal(network));
                yield return this._authenticatedHubContext.Clients.Group(this._groupNameGenerator.GenerateGlobal(network));
            }
        }
    }
}