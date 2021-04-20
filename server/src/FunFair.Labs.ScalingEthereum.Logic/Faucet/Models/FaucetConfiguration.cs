using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Logic.Faucet.Models
{
    /// <summary>
    ///     Faucet configuration.
    /// </summary>
    public sealed class FaucetConfiguration : IFaucetConfiguration
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ethToGive">The amount of ETH to issue.</param>
        /// <param name="tokenToGive">The amount of BAN to issue.</param>
        public FaucetConfiguration(EthereumAmount ethToGive, Token tokenToGive)
        {
            this.EthToGive = ethToGive;
            this.TokenToGive = tokenToGive;
        }

        /// <inheritdoc />
        public Token TokenToGive { get; }

        /// <inheritdoc />
        public EthereumAmount EthToGive { get; }
    }
}