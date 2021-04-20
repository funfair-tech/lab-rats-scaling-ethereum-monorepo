using System;
using System.Data;
using System.IO;
using Dapper;
using FunFair.Common.Data.Builders;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.TypeHandlers
{
    /// <summary>
    ///     Dapper type mapper for <see cref="Token" /> objects.
    /// </summary>
    public sealed class TokenHandler : SqlMapper.TypeHandler<Token>, IHandlerMetadata
    {
        /// <inheritdoc />
        public Type SerializeAsType => typeof(long);

        /// <inheritdoc />
        public long MaximumLength => Token.MaximumStringLength;

        /// <inheritdoc />
        public override void SetValue(IDbDataParameter parameter, Token value)
        {
            parameter.Value = (long) value.TokenAmount.Value;
        }

        /// <inheritdoc />
        public override Token Parse(object value)
        {
            switch (value)
            {
                case long longValue: return ParseLong(longValue);
                case string stringValue: return ParseString(stringValue);
                case byte[] byteValue: return ParseBytes(byteValue);
                default: throw new InvalidDataException();
            }
        }

        private static Token ParseLong(in long longValue)
        {
            return new(longValue);
        }

        private static Token ParseBytes(byte[] byteValue)
        {
            return new(byteValue);
        }

        private static Token ParseString(string stringValue)
        {
            if (!Token.TryParse(source: stringValue, out Token? value))
            {
                throw new InvalidDataException();
            }

            return value;
        }
    }
}