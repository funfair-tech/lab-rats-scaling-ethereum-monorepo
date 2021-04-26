using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;

namespace FunFair.Labs.ScalingEthereum.Logic.Games
{
    /// <summary>
    ///     Calculator of times based off of block time and network time.
    /// </summary>
    public interface IGameRoundTimeCalculator
    {
        /// <summary>
        ///     Calculates the number of seconds left in a game.
        /// </summary>
        /// <param name="gameRound">The round.</param>
        /// <returns>The number of seconds left.</returns>
        int CalculateTimeLeft(GameRound gameRound);

        /// <summary>
        ///     Calculates the number of seconds until the next round should start
        /// </summary>
        /// <param name="gameRound">The round</param>
        /// <returns>The number of seconds until the next round starts, or 0 if the round hasn't been closed</returns>
        int CalculateSecondsUntilNextRound(GameRound gameRound);
    }
}