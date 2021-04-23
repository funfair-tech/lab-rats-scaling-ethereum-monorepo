using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Logic.Alerts.Models
{
    public interface IHouseBalanceConfiguration
    {
        /// <summary>
        ///     Minimum balance in the network's native currency.
        /// </summary>
        EthereumAmount MinimumAllowedNativeCurrencyBalance { get; }
    }
}