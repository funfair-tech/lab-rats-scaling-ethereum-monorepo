using System.Diagnostics.CodeAnalysis;
using FunFair.Common.TypeConverters.Json;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.TypeConverters.Json
{
    /// <summary>
    ///     Converts a <see cref="Token" /> to/from a JSON value.
    /// </summary>
    public sealed class TokenConverter : ConverterBase<Token>
    {
        /// <inheritdoc />
        protected override bool TryParse(string? source, [NotNullWhen(true)] out Token? converted)
        {
            return Token.TryParse(source: source, value: out converted);
        }
    }
}