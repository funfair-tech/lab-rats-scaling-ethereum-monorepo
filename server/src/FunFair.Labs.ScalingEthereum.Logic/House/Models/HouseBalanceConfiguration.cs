using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Logic.House.Models
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
            /// <param name="minimumAllowedBalance">Minimum balance.</param>
            public HouseBalanceConfiguration(EthereumAmount minimumAllowedBalance)
            {
                this.MinimumAllowedNativeCurrencyBalance = minimumAllowedBalance;
            }

            /// <inheritdoc />
            public EthereumAmount MinimumAllowedNativeCurrencyBalance { get; }
        }
    }
}