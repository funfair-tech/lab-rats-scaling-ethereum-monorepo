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
        private static ContractAddress RatTrace { get; } = new("0x56422f16F3d35e51b050EC200ABB4D65F5f6d12c");

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
            return new[] {RatTrace};
        }
    }
}