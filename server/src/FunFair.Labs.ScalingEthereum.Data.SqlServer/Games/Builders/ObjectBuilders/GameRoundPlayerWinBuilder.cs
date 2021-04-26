using FunFair.Common.Data.Builders;
using FunFair.Common.Data.Extensions;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders
{
    /// <summary>
    ///     Builder of <see cref="GameRoundPlayerWinBuilder" /> from, <see cref="GameRoundPlayerWin" /> objects.
    /// </summary>
    public sealed class GameRoundPlayerWinBuilder : IObjectBuilder<GameRoundPlayerWinEntity, GameRoundPlayerWin>
    {
        /// <inheritdoc />
        public GameRoundPlayerWin? Build(GameRoundPlayerWinEntity? source)
        {
            if (source == null)
            {
                return null;
            }

            return new GameRoundPlayerWin(source.GameRoundId ?? source.DataError(x => x.GameRoundId),
                                          source.AccountAddress ?? source.DataError(x => x.AccountAddress),
                                          source.WinAmount ?? source.DataError(x => x.WinAmount),
                                          dateCreated: source.DateCreated);
        }
    }
}