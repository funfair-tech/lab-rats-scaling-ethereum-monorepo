using System.Diagnostics.CodeAnalysis;

namespace FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound
{
    /// <summary>
    ///     The game round status
    /// </summary>
    public enum GameRoundStatus
    {
        /// <summary>
        ///     New round transaction submitted, but not yet mined.
        /// </summary>
        PENDING = 0,

        /// <summary>
        ///     Round has started, and officially accepting bets
        /// </summary>
        STARTED = 1,

        /// <summary>
        ///     Betting is being stopped (stop betting tx submitted, but not yet mined).
        /// </summary>
        BETTING_STOPPING = 2,

        /// <summary>
        ///     Betting phase is over, but round not yet completed.
        /// </summary>
        BETTING_OVER = 3,

        /// <summary>
        ///     End round transaction submitted, but not yet mined.
        /// </summary>
        COMPLETING = 4,

        /// <summary>
        ///     Round has completed and no more bets can be made.  All payments have occured.
        /// </summary>
        COMPLETED = 5,

        /// <summary>
        ///     Round is broken and cannot be resumed.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Valid status, but not explicitly in code")]
        BROKEN = 6
    }
}