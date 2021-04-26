using FunFair.Common.Data.Builders;
using FunFair.Common.Data.Extensions;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders
{
    /// <summary>
    ///     Builder for <see cref="GameHistory" /> from <see cref="GameHistoryEntity" />
    /// </summary>
    public sealed class GameHistoryBuilder : IObjectBuilder<GameHistoryEntity, GameHistory>
    {
        /// <inheritdoc />
        public GameHistory? Build(GameHistoryEntity? source)
        {
            if (source == null)
            {
                return null;
            }

            return new GameHistory(dateClosed: source.DateClosed,
                                   result: source.Result ?? source.DataError(x => x.Result),
                                   history: source.History ?? source.DataError(x => x.History),
                                   gameRoundId: source.GameRoundId ?? source.DataError(x => x.GameRoundId));
        }
    }
}