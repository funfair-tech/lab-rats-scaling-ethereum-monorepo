using System;

namespace FunFair.Labs.ScalingEthereum.Logic.Games
{
    /// <summary>
    ///     The game round parameters
    /// </summary>
    public static class GameRoundParameters
    {
        /// <summary>
        ///     Length of a round.
        /// </summary>
        public static TimeSpan RoundDuration { get; } = TimeSpan.FromSeconds(value: 10);

        /// <summary>
        ///     Length of the betting close period.
        /// </summary>
        public static TimeSpan BettingCloseDuration { get; } = TimeSpan.FromSeconds(value: 1);

        /// <summary>
        ///     The amount of time to wait between rounds, so the client has time to display
        ///     the exciting results animation!
        /// </summary>
        public static TimeSpan InterGameDelay { get; } = TimeSpan.FromSeconds(value: 15);

        /// <summary>
        ///     Length of time before a round timeout can occur.
        /// </summary>
        public static TimeSpan RoundTimeoutDuration { get; } = (RoundDuration + BettingCloseDuration) * 5;
    }
}