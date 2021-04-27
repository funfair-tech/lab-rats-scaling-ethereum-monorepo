using System;
using System.Data;
using System.IO;
using Dapper;
using FunFair.Common.Data.Builders;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.TypeHandlers
{
    /// <summary>
    ///     Dapper type mapper for <see cref="WinLoss" /> objects.
    /// </summary>
    public sealed class WinLossHandler : SqlMapper.TypeHandler<WinLoss>, IHandlerMetadata
    {
        /// <inheritdoc />
        public Type SerializeAsType => typeof(string);

        /// <inheritdoc />
        public long MaximumLength => WinLoss.MaximumStringLength;

        /// <inheritdoc />
        public override void SetValue(IDbDataParameter parameter, WinLoss value)
        {
            parameter.Value = value.ToString();
        }

        /// <inheritdoc />
        public override WinLoss Parse(object value)
        {
            switch (value)
            {
                case long longValue: return ParseLong(longValue);
                case string stringValue: return ParseString(stringValue);
                case byte[] byteValue: return ParseBytes(byteValue);
                default: throw new InvalidDataException();
            }
        }

        private static WinLoss ParseLong(in long longValue)
        {
            return new(longValue);
        }

        private static WinLoss ParseBytes(byte[] byteValue)
        {
            return new(byteValue);
        }

        private static WinLoss ParseString(string stringValue)
        {
            if (!WinLoss.TryParse(source: stringValue, out WinLoss? value))
            {
                throw new InvalidDataException();
            }

            return value;
        }
    }
}