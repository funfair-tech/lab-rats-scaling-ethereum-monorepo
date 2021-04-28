using System.Diagnostics.CodeAnalysis;
using FunFair.Common.TypeConverters.Json;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.TypeConverters.Json
{
    /// <summary>
    ///     Converts a <see cref="GameRoundId" /> to/from a JSON value.
    /// </summary>
    public sealed class GameRoundIdConverter : ConverterBase<GameRoundId>
    {
        /// <inheritdoc />
        protected override bool TryParse(string? source, [NotNullWhen(true)] out GameRoundId? converted)
        {
            return GameRoundId.TryParse(source: source, value: out converted);
        }
    }
}