using System;
using System.Collections.Generic;
using FunFair.Ethereum.DataTypes;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Subscription;
using Microsoft.AspNetCore.SignalR;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Publishers
{
    /// <summary>
    ///     Base class for publishers
    /// </summary>
    public abstract class PublisherBase
    {
        private readonly IHubContext<AuthenticatedHub, IHub> _authenticatedHubContext;
        private readonly IGroupNameGenerator _groupNameGenerator;
        private readonly IHubContext<PublicHub, IHub> _publicHubContext;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="authenticatedHubContext">Authenticated hub context.</param>
        /// <param name="publicHubContext">Public hub context.</param>
        /// <param name="groupNameGenerator">Group generator.</param>
        protected PublisherBase(IHubContext<AuthenticatedHub, IHub> authenticatedHubContext, IHubContext<PublicHub, IHub> publicHubContext, IGroupNameGenerator groupNameGenerator)
        {
            this._authenticatedHubContext = authenticatedHubContext ?? throw new ArgumentNullException(nameof(authenticatedHubContext));
            this._publicHubContext = publicHubContext ?? throw new ArgumentNullException(nameof(publicHubContext));
            this._groupNameGenerator = groupNameGenerator ?? throw new ArgumentNullException(nameof(groupNameGenerator));
        }

        protected IEnumerable<IHub> GetAllHubs(EthereumNetwork network, bool includeLocalGroups, bool includeGlobalGroups)
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