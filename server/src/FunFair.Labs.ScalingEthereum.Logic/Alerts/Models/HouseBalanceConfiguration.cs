using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Logic.Alerts.Models
{
    namespace FunFair.Labs.MultiPlayer.Logic.Alerts.Models
    {
        /// <summary>
        ///     Configuration for house balances.
        /// </summary>
        public sealed class HouseBalanceConfiguration : IHouseBalanceConfiguration
        {
            /// <summary>
            ///     Constructor.
            /// </summary>
            /// <param name="minimumAllowedXdaiBalance">Minimum XDAI balance.</param>
            public HouseBalanceConfiguration(EthereumAmount minimumAllowedXdaiBalance)
            {
                this.MinimumAllowedXdaiBalance = minimumAllowedXdaiBalance;
            }

            /// <inheritdoc />
            public EthereumAmount MinimumAllowedXdaiBalance { get; }
        }
    }
}