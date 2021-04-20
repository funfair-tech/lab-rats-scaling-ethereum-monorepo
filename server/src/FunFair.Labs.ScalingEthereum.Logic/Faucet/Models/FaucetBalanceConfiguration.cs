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
        /// <param name="minimumAllowedXdaiBalance">Minimum XDAI balance.</param>
        /// <param name="minimumAllowedTokenBalance">Minimum TOKEN balance.</param>
        public FaucetBalanceConfiguration(EthereumAmount minimumAllowedXdaiBalance, TokenAmount minimumAllowedTokenBalance)
        {
            this.MinimumAllowedXdaiBalance = minimumAllowedXdaiBalance;
            this.MinimumAllowedTokenBalance = minimumAllowedTokenBalance;
        }

        /// <inheritdoc />
        public EthereumAmount MinimumAllowedXdaiBalance { get; }

        /// <inheritdoc />
        public TokenAmount MinimumAllowedTokenBalance { get; }
    }
}