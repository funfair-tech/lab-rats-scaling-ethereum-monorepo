using FunFair.Ethereum.Contracts;
using FunFair.Ethereum.Contracts.Erc20;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Contracts.Networks;

namespace FunFair.Labs.ScalingEthereum.Contracts.Token
{
    /// <summary>
    ///     The token contract.
    /// </summary>
    public static class TokenContract
    {
        /// <summary>
        ///     Creates the token.
        /// </summary>
        /// <returns></returns>
        public static Erc20TokenContractInfo Create()
        {
            // TODO: Add contract addresses here once deployed
            return ContractInfoBuilder.Create(WellKnownContracts.Token)
                                      .Network(network: Layer2EthereumNetworks.OptimismKovan, new ContractAddress("0x2222222222222222222222222222222222222222"))
                                      .BuildErc20Token(symbol: "SE667", decimalPlaces: 8);
        }
    }
}