using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Logic.Alerts.Models
{
    public interface IHouseBalanceConfiguration
    {
        /// <summary>
        ///     Minimum XDAI balance.
        /// </summary>
        EthereumAmount MinimumAllowedXdaiBalance { get; }
    }
}