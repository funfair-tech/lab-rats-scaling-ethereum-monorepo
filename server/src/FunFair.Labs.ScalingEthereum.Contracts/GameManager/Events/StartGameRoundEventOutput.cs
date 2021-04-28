using System;
using System.Diagnostics;
using FunFair.Ethereum.Contracts.Attributes;
using FunFair.Ethereum.Contracts.Events;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Events
{
    /// <summary>
    ///     Event parameters for <see cref="StartGameRoundEvent" />
    /// </summary>
    [DebuggerDisplay("Game Round Id {" + nameof(GameRoundId) + "}")]
    public sealed class StartGameRoundEventOutput : EventOutput, IGameRoundEventOutput
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="gameRoundId">Game round id.</param>
        /// <param name="persistentGameDataId">The persistent game data id.</param>
        public StartGameRoundEventOutput([EventOutputParameter(ethereumDataType: "bytes32", order: 1, indexed: true)]
                                         GameRoundId gameRoundId,
                                         [EventOutputParameter(ethereumDataType: "bytes32", order: 2, indexed: true)]
                                         byte[] persistentGameDataId)
        {
            this.GameRoundId = gameRoundId ?? throw new ArgumentNullException(nameof(gameRoundId));
            this.PersistentGameDataId = persistentGameDataId ?? throw new ArgumentNullException(nameof(persistentGameDataId));
        }

        public byte[] PersistentGameDataId { get; }

        /// <inheritdoc />
        public GameRoundId GameRoundId { get; }
    }
}