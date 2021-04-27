using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using FunFair.Ethereum.Balances.Interfaces.EventArguments;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Logic.House;

namespace FunFair.Labs.ScalingEthereum.Logic.Balances.Services
{
    /// <summary>
    ///     Watching accounts if they have enough balance to start transactions
    /// </summary>
    public sealed class LowBalanceWatcher : ILowBalanceWatcher, IDisposable
    {
        private readonly ConcurrentDictionary<NetworkAccount, bool> _houseAccounts;
        private readonly IHouseBalanceConfiguration _houseBalanceConfiguration;
        private readonly IDisposable _subscription;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="houseAccountAlerter">House account alerter</param>
        /// <param name="houseBalanceConfiguration">House balance configuration</param>
        public LowBalanceWatcher(IHouseAccountAlerter houseAccountAlerter, IHouseBalanceConfiguration houseBalanceConfiguration)
        {
            if (houseAccountAlerter == null)
            {
                throw new ArgumentNullException(nameof(houseAccountAlerter));
            }

            this._houseBalanceConfiguration = houseBalanceConfiguration ?? throw new ArgumentNullException(nameof(houseBalanceConfiguration));
            this._houseAccounts = new ConcurrentDictionary<NetworkAccount, bool>();

            this._subscription = Observable.FromEventPattern<EthereumBalanceChangeEventArgs>(addHandler: h => houseAccountAlerter.OnEthereumBalanceChanged += h,
                                                                                             removeHandler: h => houseAccountAlerter.OnEthereumBalanceChanged -= h)
                                           .Select(e => e.EventArgs)
                                           .Subscribe(e => this.UpdateBalanceStatus((NetworkAccount) e.Account, ethereumAmount: e.NewBalance));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this._subscription.Dispose();
        }

        /// <inheritdoc />
        public bool DoesAccountHaveEnoughBalance(INetworkAccount networkAccount)
        {
            return this._houseAccounts.TryGetValue(new NetworkAccount(network: networkAccount.Network, address: networkAccount.Address), out bool isEnoughBalance) && isEnoughBalance;
        }

        private void UpdateBalanceStatus(NetworkAccount networkAccount, EthereumAmount ethereumAmount)
        {
            bool enoughBalance = ethereumAmount >= this._houseBalanceConfiguration.MinimumAllowedNativeCurrencyBalance;
            this._houseAccounts.AddOrUpdate(key: networkAccount, addValue: enoughBalance, updateValueFactory: (_, _) => enoughBalance);
        }
    }
}