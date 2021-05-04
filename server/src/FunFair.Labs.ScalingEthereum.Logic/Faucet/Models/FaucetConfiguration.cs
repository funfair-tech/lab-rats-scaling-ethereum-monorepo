using System;
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
        /// <param name="nativeCurrencyToGive">The amount of the native currency to issue.</param>
        /// <param name="tokenToGive">The amount of the token to issue.</param>
        public FaucetConfiguration(EthereumAmount nativeCurrencyToGive, Token tokenToGive)
        {
            this.NativeCurrencyToGive = nativeCurrencyToGive ?? throw new ArgumentNullException(nameof(nativeCurrencyToGive));
            this.TokenToGive = tokenToGive ?? throw new ArgumentNullException(nameof(tokenToGive));
        }

        /// <inheritdoc />
        public Token TokenToGive { get; }

        /// <inheritdoc />
        public EthereumAmount NativeCurrencyToGive { get; }
    }
}