using System.Diagnostics.CodeAnalysis;
using FunFair.Common.TypeConverters.Json;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.TypeConverters.Json
{
    /// <summary>
    ///     Converts a <see cref="Seed" /> to/from a JSON value.
    /// </summary>
    public sealed class SeedConverter : ConverterBase<Seed>
    {
        /// <inheritdoc />
        protected override bool TryParse(string source, [NotNullWhen(returnValue: true)] out Seed? converted)
        {
            return Seed.TryParse(source: source, value: out converted);
        }
    }
}