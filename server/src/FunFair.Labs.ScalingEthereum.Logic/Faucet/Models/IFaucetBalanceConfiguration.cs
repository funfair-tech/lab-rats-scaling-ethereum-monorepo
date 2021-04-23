using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Logic.Faucet.Models
{
    /// <summary>
    ///     Faucet balance configuration settings.
    /// </summary>
    public interface IFaucetBalanceConfiguration
    {
        /// <summary>
        ///     Minimum native currency balance.
        /// </summary>
        EthereumAmount MinimumAllowedNativeCurrencyBalance { get; }

        /// <summary>
        ///     Minimum FUN balance.
        /// </summary>
        TokenAmount MinimumAllowedTokenBalance { get; }
    }
}