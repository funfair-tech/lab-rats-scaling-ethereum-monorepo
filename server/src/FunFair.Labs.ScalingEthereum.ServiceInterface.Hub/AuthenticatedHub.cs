using System;
using System.Threading.Tasks;
using FunFair.Common.Environment;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Labs.ScalingEthereum.Authentication;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Logic.Players;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.RateLimit;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub
{
    /// <summary>
    ///     Authenticated hub
    /// </summary>
    [Authorize(Roles = Role.Dapp)]
    public sealed class AuthenticatedHub : HubBase
    {
        private readonly IPlayerCountManager _playerCountManager;
        private readonly IRateLimiter _rateLimiter;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="ethereumNetworkRegistry">Ethereum network registry</param>
        /// <param name="groupNameGenerator">The group name generator</param>
        /// <param name="environment">Execution environment</param>
        /// <param name="subscriptionManager">Subscription manager</param>
        /// <param name="playerCountManager">Player counter.</param>
        /// <param name="rateLimiter">Rate limiter</param>
        /// <param name="logger">Logger</param>
        public AuthenticatedHub(IEthereumNetworkRegistry ethereumNetworkRegistry,
                                IGroupNameGenerator groupNameGenerator,
                                ExecutionEnvironment environment,
                                ISubscriptionManager subscriptionManager,
                                IPlayerCountManager playerCountManager,
                                IRateLimiter rateLimiter,
                                ILogger<AuthenticatedHub> logger)
            : base(ethereumNetworkRegistry: ethereumNetworkRegistry, groupNameGenerator: groupNameGenerator, environment: environment, subscriptionManager: subscriptionManager, logger: logger)
        {
            this._playerCountManager = playerCountManager ?? throw new ArgumentNullException(nameof(playerCountManager));
            this._rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
        }

        /// <summary>
        ///     Subscribe
        /// </summary>
        /// <param name="networkId">The network id</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [HubMethodName(name: HubMethodNames.Subscribe)]
        public async Task SubscribeAsync(int networkId)
        {
            EthereumNetwork network = this.VerifyNetwork(networkId);

            UserAccountId userAccountId = this.JwtUser()
                                              .Id;

            string connectionId = this.Context.ConnectionId;
            this.Logger.LogInformation($"Client {connectionId} subscribing to network {network.Name}");

            // temporary for dummy data
            await this.SubscribeNetworkAsync(connectionId: this.Context.ConnectionId, network: network);

            this.Logger.LogInformation($"Client {connectionId} subscribed to network {network.Name} and user account id {userAccountId}");

            int playerCount = await this._playerCountManager.AddPlayerAsync(playerId: this.Context.ConnectionId, newNetwork: network);

            await this.Clients.Caller.PlayersOnline(playerCount);
        }

        /// <summary>
        ///     Send message
        /// </summary>
        /// <returns></returns>
        [HubMethodName(name: HubMethodNames.SendMessage)]
        public Task SendMessageAsync(string message)
        {
            AccountAddress accountAddress = this.JwtUser()
                                                .AccountAddress;

            if (!this._rateLimiter.ShouldLimit(this.Context))
            {
                return this.Clients.All.NewMessage(accountAddress: accountAddress, message: message);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Disconnect from hub and remove player from count
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);

            int playerCount = await this._playerCountManager.RemovePlayerAsync(this.Context.ConnectionId);

            await this.Clients.Caller.PlayersOnline(playerCount);
        }
    }
}