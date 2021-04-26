using System.Collections.Generic;
using System.Linq;
using FunFair.Common.DataTypes.Helpers;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.Services
{
    /// <summary>
    ///     Builder of Start round Game History
    /// </summary>
    public sealed class StartRoundGameHistoryBuilder : IStartRoundGameHistoryBuilder
    {
        /// <inheritdoc />
        public IReadOnlyList<string> Build(IReadOnlyList<GameHistory> history)

        {
            return history.Select(h => HexEncodedString.Create(h.History))
                          .Distinct()
                          .ToArray();
        }
    }
}