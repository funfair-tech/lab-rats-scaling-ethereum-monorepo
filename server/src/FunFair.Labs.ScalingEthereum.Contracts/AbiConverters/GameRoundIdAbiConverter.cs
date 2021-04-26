using System;
using FunFair.Ethereum.Abi.Services;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.AbiConverters
{
    /// <summary>
    ///     Converts a <see cref="GameRoundId" /> into an ethereum bytes32.
    /// </summary>
    public sealed class GameRoundIdAbiConverter : Bytes32AbiConverterBase<GameRoundId>
    {
        /// <inheritdoc />
        protected override ReadOnlyMemory<byte> Encode(in GameRoundId value)
        {
            return value.ToMemory();
        }

        /// <inheritdoc />
        protected override GameRoundId Decode(in ReadOnlySpan<byte> encoded)
        {
            return new(encoded);
        }
    }
}