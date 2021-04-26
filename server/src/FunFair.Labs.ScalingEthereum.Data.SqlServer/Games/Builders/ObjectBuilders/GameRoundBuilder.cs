using System;
using FunFair.Common.Data.Builders;
using FunFair.Common.Data.Extensions;
using FunFair.Common.Extensions;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Exceptions;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders
{
    /// <summary>
    ///     Builder for <see cref="GameRound" /> from <see cref="GameRoundEntity" />
    /// </summary>
    public sealed class GameRoundBuilder : IObjectBuilder<GameRoundEntity, GameRound>
    {
        private readonly IEthereumNetworkRegistry _ethereumNetworkRegistry;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ethereumNetworkRegistry">Ethereum network registry.</param>
        public GameRoundBuilder(IEthereumNetworkRegistry ethereumNetworkRegistry)
        {
            this._ethereumNetworkRegistry = ethereumNetworkRegistry ?? throw new ArgumentNullException(nameof(ethereumNetworkRegistry));
        }

        /// <inheritdoc />
        public GameRound? Build(GameRoundEntity? source)
        {
            if (source == null)
            {
                return null;
            }

            if (!this._ethereumNetworkRegistry.TryGetByName(source.Network ?? source.DataError(x => x.Network), out EthereumNetwork? network))
            {
                throw new InvalidEthereumNetworkException();
            }

            string status = source.Status ?? source.DataError(x => x.Status);

            ContractAddress gameContract = source.GameContract ?? source.DataError(x => x.GameContract);

            return new GameRound(createdByAccount: source.CreatedByAccount ?? source.DataError(x => x.CreatedByAccount),
                                 gameRoundId: source.GameRoundId ?? source.DataError(x => x.GameRoundId),
                                 gameContract: new NetworkContract(network: network, contractAddress: gameContract),
                                 seedCommit: source.SeedCommit ?? source.DataError(x => x.SeedCommit),
                                 seedReveal: source.SeedReveal ?? source.DataError(x => x.SeedReveal),
                                 status: status.ToEnum<GameRoundStatus>(),
                                 roundDuration: TimeSpan.FromSeconds(source.RoundDuration),
                                 roundTimeoutDuration: TimeSpan.FromSeconds(source.RoundTimeoutDuration),
                                 dateCreated: source.DateCreated,
                                 dateUpdated: source.DateUpdated,
                                 dateClosed: source.DateClosed,
                                 dateStarted: source.DateStarted,
                                 blockNumberCreated: source.BlockNumberCreated ?? source.DataError(x => x.BlockNumberCreated));
        }
    }
}