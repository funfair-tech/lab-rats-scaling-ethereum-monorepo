using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Logic.Faucet.Models
{
    public interface IFaucetBalanceConfiguration
    {
        /// <summary>
        ///     Minimum XDAI balance.
        /// </summary>
        EthereumAmount MinimumAllowedXdaiBalance { get; }

        /// <summary>
        ///     Minimum FUN balance.
        /// </summary>
        TokenAmount MinimumAllowedTokenBalance { get; }
    }
}