using System.Numerics;
using FunFair.Ethereum.Abi.Services;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.AbiConverters
{
    /// <summary>
    ///     Abi Converter to convert from <see cref="WinLoss" /> to a Int256
    /// </summary>
    public sealed class WinLossAbiConverter : Int256AbiConverterBase<WinLoss>
    {
        /// <inheritdoc />
        protected override BigInteger Encode(in WinLoss value)
        {
            return value.Value;
        }

        /// <inheritdoc />
        protected override WinLoss Decode(in BigInteger encoded)
        {
            return new(encoded);
        }
    }
}