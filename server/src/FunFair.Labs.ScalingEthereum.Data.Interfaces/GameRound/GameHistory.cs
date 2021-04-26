using System;
using System.Diagnostics;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound
{
    /// <summary>
    ///     Game history.
    /// </summary>
    [DebuggerDisplay("{GameRoundId}: {DateClosed}")]
    public sealed class GameHistory
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="gameRoundId">Game round id.</param>
        /// <param name="result">Game result.</param>
        /// <param name="history">Game History.</param>
        /// <param name="dateClosed">Date and time the game was closed.</param>
        public GameHistory(GameRoundId gameRoundId, byte[] result, byte[] history, DateTime dateClosed)
        {
            this.GameRoundId = gameRoundId ?? throw new ArgumentNullException(nameof(gameRoundId));
            this.Result = result ?? throw new ArgumentNullException(nameof(result));
            this.History = history ?? throw new ArgumentNullException(nameof(history));
            this.DateClosed = dateClosed;
        }

        /// <summary>
        ///     Game round id.
        /// </summary>
        public GameRoundId GameRoundId { get; }

        /// <summary>
        ///     Game result.
        /// </summary>
        public byte[] Result { get; }

        /// <summary>
        ///     Game History.
        /// </summary>
        public byte[] History { get; }

        /// <summary>
        ///     Date and time the game was closed.
        /// </summary>
        public DateTime DateClosed { get; }
    }
}