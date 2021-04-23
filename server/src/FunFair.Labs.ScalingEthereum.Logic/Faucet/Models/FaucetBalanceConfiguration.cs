using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Logic.Faucet.Models
{
    /// <summary>
    ///     Configuration for faucet balances.
    /// </summary>
    public sealed class FaucetBalanceConfiguration : IFaucetBalanceConfiguration
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="minimumAllowedNativeCurrencyBalance">Minimum balance in the native network currency.</param>
        /// <param name="minimumAllowedTokenBalance">Minimum TOKEN balance.</param>
        public FaucetBalanceConfiguration(EthereumAmount minimumAllowedNativeCurrencyBalance, TokenAmount minimumAllowedTokenBalance)
        {
            this.MinimumAllowedNativeCurrencyBalance = minimumAllowedNativeCurrencyBalance;
            this.MinimumAllowedTokenBalance = minimumAllowedTokenBalance;
        }

        /// <inheritdoc />
        public EthereumAmount MinimumAllowedNativeCurrencyBalance { get; }

        /// <inheritdoc />
        public TokenAmount MinimumAllowedTokenBalance { get; }
    }
}