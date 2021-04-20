using System;
using System.Data;
using System.IO;
using Dapper;
using FunFair.Common.Data.Builders;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.TypeHandlers
{
    /// <summary>
    ///     Type Handler for Serializing <see cref="Seed" /> properties to/from the database.
    /// </summary>
    public sealed class SeedHandler : SqlMapper.TypeHandler<Seed>, IHandlerMetadata
    {
        /// <inheritdoc />
        public Type SerializeAsType => typeof(string);

        /// <inheritdoc />
        public long MaximumLength => Seed.RequiredStringLength;

        /// <inheritdoc />
        public override void SetValue(IDbDataParameter parameter, Seed value)
        {
            parameter.Value = value.ToString();
        }

        /// <inheritdoc />
        public override Seed Parse(object value)
        {
            switch (value)
            {
                case string stringValue: return ParseString(stringValue);
                case byte[] byteValue: return ParseBytes(byteValue);
                default: throw new InvalidDataException();
            }
        }

        private static Seed ParseBytes(byte[] byteValue)
        {
            return new(byteValue);
        }

        private static Seed ParseString(string stringValue)
        {
            if (!Seed.TryParse(source: stringValue, out Seed? value))
            {
                throw new InvalidDataException();
            }

            return value;
        }
    }
}