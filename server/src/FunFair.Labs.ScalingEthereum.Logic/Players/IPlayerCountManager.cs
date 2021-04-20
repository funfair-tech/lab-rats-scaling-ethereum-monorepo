using System.Threading.Tasks;
using FunFair.Common.Services;
using FunFair.Ethereum.DataTypes;

namespace FunFair.Labs.ScalingEthereum.Logic.Players
{
    /// <summary>
    ///     Player count manager
    /// </summary>
    public interface IPlayerCountManager : IHostedBackgroundService
    {
        /// <summary>
        ///     Add player to count
        /// </summary>
        /// <param name="playerId">Player Id</param>
        /// <param name="newNetwork">New network where player will play games</param>
        /// <returns>returns new player count </returns>
        Task<int> AddPlayerAsync(string playerId, EthereumNetwork newNetwork);

        /// <summary>
        ///     Remove player from count
        /// </summary>
        /// <param name="playerId">Player Id</param>
        /// <returns>returns new player count </returns>
        Task<int> RemovePlayerAsync(string playerId);

        /// <summary>
        ///     Get player count for network.
        /// </summary>
        /// <param name="network">Ethereum network.</param>
        /// <returns></returns>
        Task<int> GetCountAsync(EthereumNetwork network);
    }
}