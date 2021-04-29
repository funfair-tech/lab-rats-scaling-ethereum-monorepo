using System;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.Networks
{
    /// <summary>
    ///     Well-known Layer F2 Ethereum networks
    /// </summary>
    public static class Layer2EthereumNetworks
    {
        /// <summary>
        ///     The Optimism Mainnet
        /// </summary>
        public static EthereumNetwork OptimismMainNet { get; } =
            new(networkId: 1337, chainId: 10, name: @"OptimismMainNet", blockFrequency: BlockFrequency.ON_DEMAND, nativeCurrency: "OETH", isProduction: true, isStandalone: false, isPublic: true,
                fixedGasPrice: GasPrice.FromGwei(1), blockExplorer: new BlockExplorer(name: "Surge",
                                                                                      new Uri("https://mainnet-l2-explorer.surge.sh/"),
                                                                                      new Uri("https://mainnet-l2-explorer.surge.sh/address/"),
                                                                                      new Uri("https://mainnet-l2-explorer.surge.sh/tx/")));

        /// <summary>
        ///     The Optimism Kovan Testnet
        /// </summary>
        public static EthereumNetwork OptimismKovan { get; } =
            new(networkId: 1337, chainId: 69, name: @"OptimismKovan", blockFrequency: BlockFrequency.ON_DEMAND, nativeCurrency: "OKOVETH", isProduction: true, isStandalone: false, isPublic: true,
                fixedGasPrice: GasPrice.FromGwei(1), blockExplorer: new BlockExplorer(name: "Surge",
                                                                                      new Uri("https://kovan-l2-explorer.surge.sh/"),
                                                                                      new Uri("https://kovan-l2-explorer.surge.sh/address/"),
                                                                                      new Uri("https://kovan-l2-explorer.surge.sh/tx/")));
    }
}