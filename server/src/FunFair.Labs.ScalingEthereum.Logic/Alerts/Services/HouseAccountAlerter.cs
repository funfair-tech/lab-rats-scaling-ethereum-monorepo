using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FunFair.Common.Services;
using FunFair.Ethereum.Balances.Interfaces;
using FunFair.Ethereum.Balances.Interfaces.EventArguments;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Ethereum.Wallet.Interfaces;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic.Alerts.Services
{
    /// <summary>
    ///     House Account Alerting
    /// </summary>
    public sealed class HouseAccountAlerter : IHouseAccountAlerter
    {
        private readonly BufferBlock<Action> _bufferBlockEvents;
        private readonly IEthereumAccountManager _ethereumAccountManager;
        private readonly IEthereumAccountBalanceWatcher _ethereumAccountWatcher;
        private readonly IEthereumNetworkConfigurationManager _ethereumNetworkConfigurationManager;

        private readonly ConcurrentDictionary<NetworkAccount, SubscriptionToken> _houseAccounts;
        private readonly ILogger _logger;

        private readonly Func<EthereumBalanceChangeEventArgs, Task> _onHouseEthBalanceChangedAsync;

        private readonly SemaphoreSlim _semaphore = new(1);

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ethereumNetworkConfigurationManager">Ethereum network configuration manager.</param>
        /// <param name="ethereumAccountManager">Ethereum Account Manager.</param>
        /// <param name="ethereumAccountWatcher">Ethereum Account Balance Watcher.</param>
        /// <param name="logger">Logging.</param>
        public HouseAccountAlerter(IEthereumNetworkConfigurationManager ethereumNetworkConfigurationManager,
                                   IEthereumAccountManager ethereumAccountManager,
                                   IEthereumAccountBalanceWatcher ethereumAccountWatcher,
                                   ILogger<HouseAccountAlerter> logger)
        {
            this._ethereumNetworkConfigurationManager = ethereumNetworkConfigurationManager ?? throw new ArgumentNullException(nameof(ethereumNetworkConfigurationManager));
            this._ethereumAccountManager = ethereumAccountManager ?? throw new ArgumentNullException(nameof(ethereumAccountManager));
            this._ethereumAccountWatcher = ethereumAccountWatcher ?? throw new ArgumentNullException(nameof(ethereumAccountWatcher));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this._houseAccounts = new ConcurrentDictionary<NetworkAccount, SubscriptionToken>();

            this._bufferBlockEvents = BufferBlockFactory.CreateForEvents();

            this._onHouseEthBalanceChangedAsync = this.OnHouseEthBalanceChangeAsync;
        }

        /// <inheritdoc />
        public async Task SubscribeAsync(CancellationToken cancellationToken)
        {
            await this._semaphore.WaitAsync(cancellationToken);

            try
            {
                HashSet<NetworkAccount> allHouseAccounts = new();

                foreach (EthereumNetwork network in this._ethereumNetworkConfigurationManager.EnabledNetworks)
                {
                    IReadOnlyCollection<INetworkAccount> accounts = this._ethereumAccountManager.GetAccounts(network);

                    await this.SubscribeToHouseAccountsAsync(accounts: accounts, allHouseAccounts: allHouseAccounts);
                }

                await this.HandleUnSubscriptionsAsync(allHouseAccounts: allHouseAccounts);
            }
            finally
            {
                this._semaphore.Release();
            }
        }

        /// <inheritdoc />
        public event EventHandler<EthereumBalanceChangeEventArgs>? OnEthereumBalanceChanged;

        private async Task SubscribeToHouseAccountsAsync(IReadOnlyCollection<INetworkAccount> accounts, HashSet<NetworkAccount> allHouseAccounts)
        {
            foreach (INetworkAccount account in accounts)
            {
                NetworkAccount accountToCheck = new(network: account.Network, address: account.Address);
                await this.EnsureSubscribedToHouseAccountAsync(accountToCheck);

                allHouseAccounts.Add(accountToCheck);
            }
        }

        private async Task EnsureSubscribedToHouseAccountAsync(NetworkAccount account)
        {
            if (!this._houseAccounts.ContainsKey(account))
            {
                IAccountBalanceSubscriptionBuilder builder = this._ethereumAccountWatcher.Create(account)
                                                                 .InterestedInEthBalanceChange(this._onHouseEthBalanceChangedAsync)
                                                                 .InterestedInEthBalanceRetrievals(this._onHouseEthBalanceChangedAsync);

                SubscriptionToken subscriptionToken = await this._ethereumAccountWatcher.SubscribeAsync(builder);
                this._houseAccounts.TryAdd(key: account, value: subscriptionToken);

                this._logger.LogInformation($"{account.Network}: {account.Address} Subscribed for eth balance updates with {subscriptionToken}");
            }
        }

        private Task OnHouseEthBalanceChangeAsync(EthereumBalanceChangeEventArgs arg)
        {
            this._bufferBlockEvents.Post(() => this.OnEthereumBalanceChanged?.Invoke(this, e: arg));

            return Task.CompletedTask;
        }

        private async Task HandleUnSubscriptionsAsync(HashSet<NetworkAccount> allHouseAccounts)
        {
            IReadOnlyList<NetworkAccount> toUnsubscribeHouse = this._houseAccounts.Keys.Where(ha => !allHouseAccounts.Contains(ha))
                                                                   .ToArray();

            foreach (NetworkAccount account in toUnsubscribeHouse)
            {
                if (this._houseAccounts.TryRemove(key: account, out SubscriptionToken? subscriptionToken))
                {
                    await this.TryUnsubscribeAsync(account: account, subscriptionToken: subscriptionToken);
                }
            }
        }

        private async Task TryUnsubscribeAsync(INetworkAccount account, SubscriptionToken subscriptionToken)
        {
            try
            {
                await this._ethereumAccountWatcher.UnsubscribeAsync(subscriptionToken);

                this._logger.LogInformation($"{account.Network}: {account.Address} Unsubscribed for balance updates with {subscriptionToken}");
            }
            catch (Exception exception)
            {
                this._logger.LogWarning(new EventId(exception.HResult),
                                        exception: exception,
                                        $"{account.Network}: {account.Address} Failed to unsubscribe with token {subscriptionToken}: {exception.Message}");
            }
        }
    }
}