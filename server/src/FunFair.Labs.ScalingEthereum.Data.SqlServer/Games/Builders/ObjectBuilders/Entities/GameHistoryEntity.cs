using System;
using System.Diagnostics;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     Game history.
    /// </summary>
    [DebuggerDisplay("{GameRoundId}: {DateClosed}")]
    public sealed record GameHistoryEntity
    {
        /// <summary>
        ///     Game round id.
        /// </summary>
        public GameRoundId? GameRoundId { get; init; }

        /// <summary>
        ///     Game result.
        /// </summary>
        public byte[]? Result { get; init; }

        /// <summary>
        ///     Game History.
        /// </summary>
        public byte[]? History { get; init; }

        /// <summary>
        ///     Date and time the game was closed.
        /// </summary>
        public DateTime DateClosed { get; init; }
    }
}