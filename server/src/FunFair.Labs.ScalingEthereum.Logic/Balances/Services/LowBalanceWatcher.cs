using System;
using System.Collections.Concurrent;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Logic.House;

namespace FunFair.Labs.ScalingEthereum.Logic.Balances.Services
{
    /// <inheritdoc />
    public sealed class LowBalanceWatcher : ILowBalanceWatcher
    {
        private readonly IHouseAccountAlerter _houseAccountAlerter;
        private readonly ConcurrentDictionary<NetworkAccount, bool> _houseAccounts;
        private readonly IHouseBalanceConfiguration _houseBalanceConfiguration;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="houseAccountAlerter">House account alerter</param>
        /// <param name="houseBalanceConfiguration">House balance configuration</param>
        public LowBalanceWatcher(IHouseAccountAlerter houseAccountAlerter, IHouseBalanceConfiguration houseBalanceConfiguration)
        {
            this._houseAccountAlerter = houseAccountAlerter ?? throw new ArgumentNullException(nameof(houseAccountAlerter));
            this._houseBalanceConfiguration = houseBalanceConfiguration ?? throw new ArgumentNullException(nameof(houseBalanceConfiguration));
            this._houseAccounts = new ConcurrentDictionary<NetworkAccount, bool>();

            this._houseAccountAlerter.OnEthereumBalanceChanged += (_, e) => this.UpdateBalanceStatus((NetworkAccount) e.Account, ethereumAmount: e.NewBalance);
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