using System;
using System.Diagnostics;
using FunFair.Ethereum.Contracts.Attributes;
using FunFair.Ethereum.Contracts.Events;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Events
{
    /// <summary>
    ///     Event parameters for <see cref="NoMoreBetsEvent" />
    /// </summary>
    [DebuggerDisplay("Game Round Id {" + nameof(GameRoundId) + "}")]
    public sealed class NoMoreBetsEventOutput : EventOutput, IGameRoundEventOutput
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="gameRoundId">Game round id.</param>
        public NoMoreBetsEventOutput([EventOutputParameter(ethereumDataType: "bytes32", order: 1, indexed: true)]
                                     GameRoundId gameRoundId)
        {
            this.GameRoundId = gameRoundId ?? throw new ArgumentNullException(nameof(gameRoundId));
        }

        /// <inheritdoc />
        public GameRoundId GameRoundId { get; }
    }
}