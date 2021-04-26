using System.Collections.Generic;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;

namespace FunFair.Labs.ScalingEthereum.Logic.Games
{
    /// <summary>
    ///     Builder of Start round Game History
    /// </summary>
    public interface IStartRoundGameHistoryBuilder
    {
        /// <summary>
        ///     Builds the game history entries.
        /// </summary>
        /// <param name="history">The game history.</param>
        /// <returns>Collection of game history.</returns>
        IReadOnlyList<string> Build(IReadOnlyList<GameHistory> history);
    }
}