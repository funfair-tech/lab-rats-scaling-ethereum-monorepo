using System;
using System.Collections.Generic;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Contracts.Networks;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.Services
{
    /// <summary>
    ///     Games list
    /// </summary>
    public sealed class GamesList : IGamesList
    {
        private static ContractAddress GameContract { get; } = new("0x11ed98eC76b2E0359639A1128766525c0dd6d6B0");

        /// <inheritdoc />
        public IReadOnlyList<ContractAddress> GetGamesForNetwork(EthereumNetwork network)
        {
            if (network == Layer2EthereumNetworks.OptimismKovan)
            {
                return OptimismKovanGames();
            }

            return Array.Empty<ContractAddress>();
        }

        private static IReadOnlyList<ContractAddress> OptimismKovanGames()
        {
            return new[] {GameContract};
        }
    }
}