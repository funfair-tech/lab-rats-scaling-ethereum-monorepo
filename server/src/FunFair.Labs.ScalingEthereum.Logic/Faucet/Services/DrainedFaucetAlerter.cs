using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Alerts.Dispatching;
using FunFair.Alerts.Interfaces;
using FunFair.Ethereum.Balances.Interfaces;
using FunFair.Ethereum.Balances.Interfaces.EventArguments;
using FunFair.Ethereum.Contracts;
using FunFair.Ethereum.Contracts.Erc20;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Labs.ScalingEthereum.Contracts;
using FunFair.Labs.ScalingEthereum.Logic.Faucet.Models;
using Microsoft.Extensions.Logging;
using NonBlocking;

namespace FunFair.Labs.ScalingEthereum.Logic.Faucet.Services
{
    /// <inheritdoc />
    public sealed class DrainedFaucetAlerter : IDrainedFaucetAlerter
    {
        private readonly IAlertDispatcher _alertDispatcher;
        private readonly IContractInfoRegistry _contractInfoRegistry;
        private readonly IEthereumAccountBalanceWatcher _ethereumAccountWatcher;

        private readonly IEthereumNetworkConfigurationManager _ethereumNetworkConfigurationManager;
        private readonly IFaucetBalanceConfiguration _faucetBalanceConfiguration;
        private readonly ILogger<DrainedFaucetAlerter> _logger;
        private readonly Erc20TokenContractInfo _tokenContract;

        private readonly ConcurrentDictionary<NetworkAccount, SubscriptionToken> _watchedContracts;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="ethereumNetworkConfigurationManager">Ethereum network configuration manager.</param>
        /// <param name="ethereumAccountWatcher">Ethereum Account Watcher.</param>
        /// <param name="contractInfoRegistry">Contract info registry</param>
        /// <param name="alertDispatcher">Alert dispatcher.</param>
        /// <param name="faucetBalanceConfiguration">Faucet balance configuration</param>
        /// <param name="logger">Logger.</param>
        public DrainedFaucetAlerter(IEthereumNetworkConfigurationManager ethereumNetworkConfigurationManager,
                                    IEthereumAccountBalanceWatcher ethereumAccountWatcher,
                                    IContractInfoRegistry contractInfoRegistry,
                                    IAlertDispatcher alertDispatcher,
                                    IFaucetBalanceConfiguration faucetBalanceConfiguration,
                                    ILogger<DrainedFaucetAlerter> logger)
        {
            this._ethereumNetworkConfigurationManager = ethereumNetworkConfigurationManager ?? throw new ArgumentNullException(nameof(ethereumNetworkConfigurationManager));
            this._ethereumAccountWatcher = ethereumAccountWatcher ?? throw new ArgumentNullException(nameof(ethereumAccountWatcher));
            this._contractInfoRegistry = contractInfoRegistry ?? throw new ArgumentNullException(nameof(contractInfoRegistry));
            this._alertDispatcher = alertDispatcher ?? throw new ArgumentNullException(nameof(alertDispatcher));
            this._faucetBalanceConfiguration = faucetBalanceConfiguration ?? throw new ArgumentNullException(nameof(faucetBalanceConfiguration));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this._tokenContract = (Erc20TokenContractInfo) contractInfoRegistry.FindContractInfo(WellKnownContracts.Token);

            this._watchedContracts = new ConcurrentDictionary<NetworkAccount, SubscriptionToken>();
        }

        /// <param name="cancellationToken"></param>
        /// <inheritdoc />
        public Task SubscribeAsync(CancellationToken cancellationToken)
        {
            IReadOnlyList<EthereumNetwork> networks = this._ethereumNetworkConfigurationManager.EnabledNetworks;

            return this.SubscribeToBalanceNotificationsForContractAsync(networks: networks, contractName: WellKnownContracts.Faucet);
        }

        private async Task SubscribeToBalanceNotificationsForContractAsync(IReadOnlyList<EthereumNetwork> networks, string contractName)
        {
            IContractInfo contractInfo = this._contractInfoRegistry.FindContractInfo(contractName);

            foreach (EthereumNetwork network in networks)
            {
                if (contractInfo.Addresses.TryGetValue(key: network, out ContractAddress? contract))
                {
                    NetworkAccount networkAccount = new(network: network, new AccountAddress(contract.ToSpan()));

                    await this.MonitorAccountAsync(networkAccount);
                }
            }
        }

        private async Task MonitorAccountAsync(NetworkAccount networkAccount)
        {
            if (!this._watchedContracts.ContainsKey(networkAccount))
            {
                SubscriptionToken subscriptionToken = await this._ethereumAccountWatcher.SubscribeAsync(this._ethereumAccountWatcher.Create(networkAccount)
                                                                                                            .InterestedInEthBalanceChange(NotifyEthBalanceChange)
                                                                                                            .InterestedInTokenBalanceChange(
                                                                                                                contract: this._tokenContract,
                                                                                                                balanceChangedCallback: NotifyTokenBalanceChange));

                this._watchedContracts.TryAdd(key: networkAccount, value: subscriptionToken);

                Task NotifyTokenBalanceChange(TokenBalanceChangeEventArgs args)
                {
                    return this.NotifyForAllTokenBalanceChangesAsync(contractAccount: networkAccount, minimumTokenAmount: this._faucetBalanceConfiguration.MinimumAllowedTokenBalance, args: args);
                }

                Task NotifyEthBalanceChange(EthereumBalanceChangeEventArgs args)
                {
                    return this.NotifyForAllEthBalanceChangesAsync(contractAccount: networkAccount, new EthereumAmount(this._faucetBalanceConfiguration.MinimumAllowedXdaiBalance.Value), args: args);
                }
            }
        }

        private async Task NotifyForAllEthBalanceChangesAsync(INetworkAccount contractAccount, EthereumAmount minimumEthereumAmount, EthereumBalanceChangeEventArgs args)
        {
            if (args.NewBalance < minimumEthereumAmount)
            {
                string message = $"{contractAccount.Network.Name}: Faucet contract at address {contractAccount.Address}  is low on XDAI";
                this._logger.LogCritical(message);
                await this._alertDispatcher.TriggerAsync(GetContractEthBalanceAlertKey(args.Account),
                                                         summary: message,
                                                         severity: EventSeverity.CRITICAL,
                                                         new Dictionary<string, string> {{"Current balance", args.NewBalance.Value.ToString()}});
            }
        }

        private async Task NotifyForAllTokenBalanceChangesAsync(INetworkAccount contractAccount, TokenAmount minimumTokenAmount, TokenBalanceChangeEventArgs args)
        {
            if (args.NewBalance.TokenAmount < minimumTokenAmount)
            {
                string message = $"{contractAccount.Network.Name}: Faucet contract at address {contractAccount.Address} is low on TOKEN";
                this._logger.LogCritical(message);
                await this._alertDispatcher.TriggerAsync(GetContractTokenBalanceAlertKey(args.Account),
                                                         summary: message,
                                                         severity: EventSeverity.CRITICAL,
                                                         new Dictionary<string, string> {{"Current balance", args.NewBalance.TokenAmount.Value.ToString()}});
            }
        }

        private static string BuildKey(string context, EthereumNetwork network, params string[] data)
        {
            return string.Join(separator: @"::", new[] {context, network.Name}.Concat(data))
                         .ToLowerInvariant();
        }

        private static string GetContractEthBalanceAlertKey(INetworkAccount account)
        {
            return BuildKey(context: @"Balance", network: account.Network, account.Address.ToString(), @"ETH");
        }

        private static string GetContractTokenBalanceAlertKey(INetworkAccount account)
        {
            return BuildKey(context: @"Balance", network: account.Network, account.Address.ToString(), @"TOKEN");
        }
    }
}