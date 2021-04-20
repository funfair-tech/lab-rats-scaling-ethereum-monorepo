using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunFair.Common.Environment;
using FunFair.Common.Environment.Extensions;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Labs.ScalingEthereum.Authentication;
using FunFair.Labs.ScalingEthereum.Authentication.Exceptions;
using FunFair.Labs.ScalingEthereum.Authentication.Extensions;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Subscription;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub
{
    public abstract class HubBase : Hub<IHub>
    {
        private readonly ExecutionEnvironment _environment;

        private readonly IGroupNameGenerator _groupNameGenerator;

        private readonly ISubscriptionManager _subscribedGroups;

        protected HubBase(IEthereumNetworkRegistry ethereumNetworkRegistry,
                          IGroupNameGenerator groupNameGenerator,
                          ExecutionEnvironment environment,
                          ISubscriptionManager subscriptionManager,
                          ILogger logger)
        {
            this.NetworkManager = ethereumNetworkRegistry ?? throw new ArgumentNullException(nameof(ethereumNetworkRegistry));
            this._groupNameGenerator = groupNameGenerator ?? throw new ArgumentNullException(nameof(groupNameGenerator));
            this._environment = environment;
            this._subscribedGroups = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected ILogger Logger { get; }

        private IEthereumNetworkRegistry NetworkManager { get; }

        /// <summary>
        ///     Handles unsubscribing hub specific items.
        /// </summary>
        protected virtual Task OnUnSubscribeAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override async Task OnConnectedAsync()
        {
            this.Logger.LogInformation($"Client {this.Context.ConnectionId} connected");

            await base.OnConnectedAsync();

            await this.LogClientAsync(connectionId: this.Context.ConnectionId, $"Connected to {Environment.MachineName}");
        }

        /// <inheritdoc />
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception != null)
            {
                this.Logger.LogError(new EventId(exception.HResult), exception: exception, $"Client {this.Context.ConnectionId} disconnected with error");
            }
            else
            {
                this.Logger.LogInformation($"Client {this.Context.ConnectionId} disconnected");
            }

            await this.OnUnSubscribeAsync();

            await this.RemoveFromAllGroupsAsync(this.Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        ///     Add a connection to a group
        /// </summary>
        /// <param name="connectionId">The connection id</param>
        /// <param name="name">The name of the group</param>
        private async Task AddToGroupAsync(string connectionId, string name)
        {
            this._subscribedGroups.Subscribe(groupName: connectionId, new List<string> {name});

            await this.Groups.AddToGroupAsync(connectionId: connectionId, groupName: name);

            if (this._environment.IsLocalDevelopmentOrTest())
            {
                await this.LogClientAsync(connectionId: connectionId, $"Adding to group {name}");
            }
        }

        /// <summary>
        ///     Remove the connection from all groups
        /// </summary>
        /// <param name="connectionId">The connection id</param>
        protected async Task RemoveFromAllGroupsAsync(string connectionId)
        {
            if (this._subscribedGroups.Unsubscribe(groupName: connectionId, out IList<string>? groups))
            {
                await this.LogClientAsync(connectionId: connectionId, $"Removing from groups {string.Join(separator: ", ", values: groups)}");

                await Task.WhenAll(groups.Select(selector: group => this.Groups.RemoveFromGroupAsync(connectionId: connectionId, groupName: group)));
            }
        }

        protected Task SubscribeAccountAsync(string connectionId, EthereumNetwork network, UserAccountId userAccountId)
        {
            if (userAccountId == null)
            {
                return Task.CompletedTask;
            }

            return Task.WhenAll(this.AddToGroupAsync(connectionId: connectionId, this._groupNameGenerator.GenerateGlobal(network: network, accountAddress: userAccountId)),
                                this.AddToGroupAsync(connectionId: connectionId, this._groupNameGenerator.GenerateLocal(network: network, accountAddress: userAccountId)));
        }

        protected Task SubscribeNetworkAsync(string connectionId, EthereumNetwork network)
        {
            return Task.WhenAll(this.AddToGroupAsync(connectionId: connectionId, this._groupNameGenerator.GenerateLocal(network)),
                                this.AddToGroupAsync(connectionId: connectionId, this._groupNameGenerator.GenerateGlobal(network)));
        }

        protected JwtUser JwtUser()
        {
            if (this.Context.User?.Identity?.IsAuthenticated != true)
            {
                throw new JwtUserNullException();
            }

            return this.Context.User.ToJwtUser();
        }

        /// <summary>
        ///     Log to the client.
        /// </summary>
        /// <param name="connectionId">The client's connection id.</param>
        /// <param name="message">The message to send to the client</param>
        protected async Task LogClientAsync(string connectionId, string message)
        {
            if (this._environment.IsLocalDevelopmentOrTest())
            {
                await this.Clients.Caller.Log(connectionId: connectionId, message: message);
            }
        }

        protected EthereumNetwork VerifyNetwork(int networkId)
        {
            // TODO: Verify this makes sense?
            if (!this.NetworkManager.TryGetByChainId(chainId: networkId, out EthereumNetwork? network))
            {
                throw new ArgumentOutOfRangeException(nameof(networkId), message: "Network is not enabled");
            }

            return network;
        }
    }
}