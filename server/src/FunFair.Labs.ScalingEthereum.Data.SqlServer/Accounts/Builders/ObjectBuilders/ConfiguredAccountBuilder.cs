using System;
using FunFair.Common.Data.Builders;
using FunFair.Common.Data.Extensions;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Accounts.Builders.ObjectBuilders.Entities;
using FunFair.Server.Ethereum.Accounts.Data.Interfaces;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Accounts.Builders.ObjectBuilders
{
    /// <summary>
    ///     Builder of <see cref="ConfiguredAccount" />
    /// </summary>
    public sealed class ConfiguredAccountBuilder : IObjectBuilder<ConfiguredAccountEntity, ConfiguredAccount>
    {
        private readonly IEthereumNetworkRegistry _ethereumNetworkRegistry;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ethereumNetworkRegistry">Ethereum network registry.</param>
        public ConfiguredAccountBuilder(IEthereumNetworkRegistry ethereumNetworkRegistry)
        {
            this._ethereumNetworkRegistry = ethereumNetworkRegistry ?? throw new ArgumentNullException(nameof(ethereumNetworkRegistry));
        }

        /// <inheritdoc />
        public ConfiguredAccount? Build(ConfiguredAccountEntity? source)
        {
            if (source == null)
            {
                return null;
            }

            if (!this._ethereumNetworkRegistry.TryGetByName(source.Network ?? source.DataError(x => x.Network), out EthereumNetwork? network))
            {
                // Silently ignore networks that aren't enabled
                return null;
            }

            return new ConfiguredAccount(network: network,
                                         accountAddress: source.AccountAddress ?? source.DataError(x => x.AccountAddress),
                                         wallet: source.Wallet ?? source.DataError(x => x.Wallet),
                                         unlock: source.Unlock ?? source.DataError(x => x.Unlock),
                                         tokenFundingAccountAddress: source.TokenFundingAccountAddress ?? source.DataError(x => x.TokenFundingAccountAddress),
                                         version: source.Version,
                                         enabled: source.Enabled);
        }
    }
}