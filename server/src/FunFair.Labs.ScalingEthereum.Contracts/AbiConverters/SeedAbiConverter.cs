using System;
using FunFair.Ethereum.Abi.Services;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.AbiConverters
{
    /// <summary>
    ///     Converts a <see cref="Seed" /> into an ethereum bytes32.
    /// </summary>
    public sealed class SeedAbiConverter : Bytes32AbiConverterBase<Seed>
    {
        /// <inheritdoc />
        protected override ReadOnlyMemory<byte> Encode(in Seed value)
        {
            return value.ToMemory();
        }

        /// <inheritdoc />
        protected override Seed Decode(in ReadOnlySpan<byte> encoded)
        {
            return new(encoded);
        }
    }
}