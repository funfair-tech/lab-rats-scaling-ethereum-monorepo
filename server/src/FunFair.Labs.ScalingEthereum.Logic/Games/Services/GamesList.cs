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
        private static ContractAddress RatTrace { get; } = new("0x1804365F80251b3c38185Eb7756B26D069D5A437");

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