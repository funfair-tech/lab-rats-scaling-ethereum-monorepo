using System.Numerics;
using FunFair.Ethereum.Abi.Services;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.AbiConverters
{
    /// <summary>
    ///     Abi Converter to convert from <see cref="Token" /> to a Uint256
    /// </summary>
    public sealed class TokenAbiConverter : UnsignedInt256AbiConverterBase<DataTypes.Primitives.Token>
    {
        /// <inheritdoc />
        protected override BigInteger Encode(in DataTypes.Primitives.Token value)
        {
            return value.TokenAmount.Value;
        }

        /// <inheritdoc />
        protected override DataTypes.Primitives.Token Decode(in BigInteger encoded)
        {
            return new(encoded);
        }
    }
}