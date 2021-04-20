using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Logic.Faucet.Models
{
    /// <summary>
    ///     Configuration for the faucet.
    /// </summary>
    public interface IFaucetConfiguration
    {
        /// <summary>
        ///     The amount of token to issue.
        /// </summary>
        Token TokenToGive { get; }

        /// <summary>
        ///     The amount of ETH to issue.
        /// </summary>
        EthereumAmount EthToGive { get; }
    }
}