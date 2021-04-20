using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;

namespace FunFair.Labs.ScalingEthereum.Logic.Players
{
    /// <summary>
    ///     Publisher or player statistics.
    /// </summary>
    public interface IPlayerStatisticsPublisher
    {
        /// <summary>
        ///     Notify the number of players online
        /// </summary>
        /// <param name="network">The network the statistics are for.</param>
        /// <param name="players">The number of players online.</param>
        Task AmountOfPlayersAsync(EthereumNetwork network, int players);
    }
}