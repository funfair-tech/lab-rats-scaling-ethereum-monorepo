using System;
using System.Data;
using System.IO;
using Dapper;
using FunFair.Common.Data.Builders;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.TypeHandlers
{
    /// <summary>
    ///     Type Handler for Serializing <see cref="GameRoundId" /> properties to/from the database.
    /// </summary>
    public sealed class GameRoundIdHandler : SqlMapper.TypeHandler<GameRoundId>, IHandlerMetadata
    {
        /// <inheritdoc />
        public Type SerializeAsType => typeof(string);

        /// <inheritdoc />
        public long MaximumLength => GameRoundId.RequiredStringLength;

        /// <inheritdoc />
        public override void SetValue(IDbDataParameter parameter, GameRoundId value)
        {
            parameter.Value = value.ToString();
        }

        /// <inheritdoc />
        public override GameRoundId Parse(object value)
        {
            switch (value)
            {
                case string stringValue: return ParseString(stringValue);
                case byte[] byteValue: return ParseBytes(byteValue);
                default: throw new InvalidDataException();
            }
        }

        private static GameRoundId ParseBytes(byte[] byteValue)
        {
            return new(byteValue);
        }

        private static GameRoundId ParseString(string stringValue)
        {
            if (!GameRoundId.TryParse(source: stringValue, out GameRoundId? value))
            {
                throw new InvalidDataException();
            }

            return value;
        }
    }
}