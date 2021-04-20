using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;

namespace FunFair.Labs.ScalingEthereum.Data.Interfaces.PlayerCount
{
    /// <summary>
    ///     The player count data manager
    /// </summary>
    public interface IPlayerCountDataManager
    {
        /// <summary>
        ///     Increment number of players.
        /// </summary>
        /// <param name="machineName">Machine name.</param>
        /// <param name="network">Ethereum network.</param>
        /// <returns></returns>
        Task<int> IncrementAsync(string machineName, EthereumNetwork network);

        /// <summary>
        ///     Decrement number of players.
        /// </summary>
        /// <param name="machineName">Machine name.</param>
        /// <param name="network">Ethereum network.</param>
        /// <returns></returns>
        Task<int> DecrementAsync(string machineName, EthereumNetwork network);

        /// <summary>
        ///     Reset player count for machine to 0.
        /// </summary>
        /// <param name="machineName">Machine name.</param>
        /// <returns></returns>
        Task ResetAsync(string machineName);

        /// <summary>
        ///     Gets the total count of online players.
        /// </summary>
        /// <param name="network">Ethereum network.</param>
        /// <returns>Total count of players.</returns>
        Task<int> GetAsync(EthereumNetwork network);
    }
}