using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FunFair.Common.Environment;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Subscription;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub
{
    /// <summary>
    ///     Public hub (no authentication)
    /// </summary>
    [SuppressMessage(category: "ReSharper", checkId: "ClassNeverInstantiated.Global", Justification = "TODO: Review")]
    public sealed class PublicHub : HubBase
    {
        /// <summary>
        ///     The public hub
        /// </summary>
        /// <param name="ethereumNetworkRegistry">Ethereum network registry</param>
        /// <param name="groupNameGenerator">The group name generator</param>
        /// <param name="environment">Execution environment</param>
        /// <param name="subscriptionManager">Subscription manager</param>
        /// <param name="logger">Logger</param>
        public PublicHub(IEthereumNetworkRegistry ethereumNetworkRegistry,
                         IGroupNameGenerator groupNameGenerator,
                         ExecutionEnvironment environment,
                         ISubscriptionManager subscriptionManager,
                         ILogger<PublicHub> logger)
            : base(ethereumNetworkRegistry: ethereumNetworkRegistry, groupNameGenerator: groupNameGenerator, environment: environment, subscriptionManager: subscriptionManager, logger: logger)
        {
        }

        /// <summary>
        ///     Subscribe
        /// </summary>
        /// <param name="networkId">Network id</param>
        /// <returns></returns>
        [HubMethodName(name: "Subscribe")]
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public Task SubscribeAsync(int networkId)
        {
            EthereumNetwork network = this.VerifyNetwork(networkId);

            return this.SubscribeNetworkAsync(connectionId: this.Context.ConnectionId, network: network);
        }
    }
}