using System.Collections.Generic;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Logic.Games
{
    /// <summary>
    ///     Games list
    /// </summary>
    public interface IGamesList
    {
        /// <summary>
        ///     Gets the installed games for the network.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <returns>List of games.</returns>
        IReadOnlyList<ContractAddress> GetGamesForNetwork(EthereumNetwork network);
    }
}