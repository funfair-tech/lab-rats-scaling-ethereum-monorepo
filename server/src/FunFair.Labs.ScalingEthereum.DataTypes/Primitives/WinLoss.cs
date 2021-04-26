using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using FunFair.Common.DataTypes;
using FunFair.Common.DataTypes.Interfaces;
using FunFair.Common.DataTypes.Strategies;
using FunFair.Common.Extensions;

namespace FunFair.Labs.ScalingEthereum.DataTypes.Primitives
{
    /// <summary>
    ///     A Win/Loss value
    /// </summary>
    [DebuggerDisplay(value: "{Value} ({ToString(), nq}")]
    public sealed class WinLoss : IHexInteger<WinLoss, BigInteger>
    {
        /// <summary>
        ///     The maximum length of the Hex Integer when formatted as a string.
        /// </summary>
        public const int MaximumStringLength = Maximum32ByteBoundsCheckedValidationStrategy.MaximumStringLength;

        private readonly ReadOnlyHexIntegerValue<BigInteger, BigIntegerByteExtractionStrategy, NoConversionStrategy, Maximum32ByteBoundsCheckedValidationStrategy,
            LeftPadBytesAndFormattedTo32ByteBoundsStrategy, PrefixedLowerCaseHexStringFormattingStrategy> _value;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="hex">The hex string to create the string from.</param>
        public WinLoss(string hex)
            : this(
                new ReadOnlyHexIntegerValue<BigInteger, BigIntegerByteExtractionStrategy, NoConversionStrategy, Maximum32ByteBoundsCheckedValidationStrategy,
                    LeftPadBytesAndFormattedTo32ByteBoundsStrategy, PrefixedLowerCaseHexStringFormattingStrategy>(hex))
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="value">The integer to create the string from.</param>
        public WinLoss(BigInteger value)
            : this(
                new ReadOnlyHexIntegerValue<BigInteger, BigIntegerByteExtractionStrategy, NoConversionStrategy, Maximum32ByteBoundsCheckedValidationStrategy,
                    LeftPadBytesAndFormattedTo32ByteBoundsStrategy, PrefixedLowerCaseHexStringFormattingStrategy>(value))
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="bytes">The bytes to create the string from.</param>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public WinLoss(byte[] bytes)
            : this(
                new ReadOnlyHexIntegerValue<BigInteger, BigIntegerByteExtractionStrategy, NoConversionStrategy, Maximum32ByteBoundsCheckedValidationStrategy,
                    LeftPadBytesAndFormattedTo32ByteBoundsStrategy, PrefixedLowerCaseHexStringFormattingStrategy>(bytes))
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="bytes">The bytes to create the string from.</param>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public WinLoss(in ReadOnlySpan<byte> bytes)
            : this(
                new ReadOnlyHexIntegerValue<BigInteger, BigIntegerByteExtractionStrategy, NoConversionStrategy, Maximum32ByteBoundsCheckedValidationStrategy,
                    LeftPadBytesAndFormattedTo32ByteBoundsStrategy, PrefixedLowerCaseHexStringFormattingStrategy>(bytes))
        {
        }

        private WinLoss(
            ReadOnlyHexIntegerValue<BigInteger, BigIntegerByteExtractionStrategy, NoConversionStrategy, Maximum32ByteBoundsCheckedValidationStrategy, LeftPadBytesAndFormattedTo32ByteBoundsStrategy,
                PrefixedLowerCaseHexStringFormattingStrategy> value)
        {
            this._value = value;
        }

        /// <summary>
        ///     A Zero Win/Loss value.
        /// </summary>
        public static WinLoss Zero { get; } = new(BigInteger.Zero);

        /// <inheritdoc />
        public bool Equals(WinLoss? other)
        {
            return AreEqual(this, h2: other);
        }

        /// <inheritdoc />
        public int CompareTo(WinLoss? other)
        {
            if (ReferenceEquals(objA: other, objB: null))
            {
                return 1;
            }

            return CompareToCommon(this, h2: other);
        }

        /// <inheritdoc />
        public byte[] ToBigEndianByteArray()
        {
            return this._value.ToBigEndianByteArray();
        }

        /// <inheritdoc />
        public BigInteger Value => this._value.Value;

        /// <inheritdoc />
        public ReadOnlySpan<byte> ToSpan()
        {
            return this._value.ToSpan();
        }

        /// <inheritdoc />
        public ReadOnlyMemory<byte> ToMemory()
        {
            return this._value.ToMemory();
        }

        /// <inheritdoc />
        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(objA: obj, objB: null))
            {
                return 1;
            }

            if (obj is WinLoss x)
            {
                return CompareToCommon(this, h2: x);
            }

            throw new ArgumentException(message: "Must be a WinLoss", nameof(obj));
        }

        /// <summary>
        ///     Attempts to create a <see cref="WinLoss" /> from the given string
        /// </summary>
        /// <param name="source">the string to parse.</param>
        /// <param name="value">The value, if it could be parsed; otherwise, null.</param>
        /// <returns>True, if the value was valid; otherwise, false.</returns>
        public static bool TryParse(string? source, [NotNullWhen(returnValue: true)] out WinLoss? value)
        {
            if (ReadOnlyHexIntegerValue.TryParseBase(source: source,
                                                     out ReadOnlyHexIntegerValue<BigInteger, BigIntegerByteExtractionStrategy, NoConversionStrategy, Maximum32ByteBoundsCheckedValidationStrategy,
                                                         LeftPadBytesAndFormattedTo32ByteBoundsStrategy, PrefixedLowerCaseHexStringFormattingStrategy> parsed))
            {
                if (Zero.Value == parsed.Value)
                {
                    value = Zero;

                    return true;
                }

                value = new WinLoss(parsed);

                return true;
            }

            value = null;

            return false;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this._value.ToString();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this._value.GetHashCode();
        }

        /// <summary>
        ///     Equality comparison via operator overload
        /// </summary>
        /// <param name="h1">The first <see cref="WinLoss" />.</param>
        /// <param name="h2">The second <see cref="WinLoss" />.</param>
        /// <returns>true, if they are the same; otherwise, false.</returns>
        public static bool operator ==(WinLoss? h1, WinLoss? h2)
        {
            return AreEqual(h1: h1, h2: h2);
        }

        /// <summary>
        ///     Inequality comparison via operator overload.
        /// </summary>
        /// <param name="h1">The first <see cref="WinLoss" />.</param>
        /// <param name="h2">The second <see cref="WinLoss" />.</param>
        /// <returns>true, if they are different; otherwise, false.</returns>
        public static bool operator !=(WinLoss? h1, WinLoss? h2)
        {
            return !AreEqual(h1: h1, h2: h2);
        }

        /// <summary>
        ///     Equality comparison via operator overload
        /// </summary>
        /// <param name="h1">The first <see cref="WinLoss" />.</param>
        /// <param name="h2">The second <see cref="WinLoss" />.</param>
        /// <returns>true, if they are the same; otherwise, false.</returns>
        public static bool operator <(WinLoss? h1, WinLoss? h2)
        {
            return CompareToCommon(h1: h1, h2: h2) < 0;
        }

        /// <summary>
        ///     Equality comparison via operator overload
        /// </summary>
        /// <param name="h1">The first <see cref="WinLoss" />.</param>
        /// <param name="h2">The second <see cref="WinLoss" />.</param>
        /// <returns>true, if they are the same; otherwise, false.</returns>
        public static bool operator <=(WinLoss? h1, WinLoss? h2)
        {
            return CompareToCommon(h1: h1, h2: h2) <= 0;
        }

        /// <summary>
        ///     Equality comparison via operator overload
        /// </summary>
        /// <param name="h1">The first <see cref="WinLoss" />.</param>
        /// <param name="h2">The second <see cref="WinLoss" />.</param>
        /// <returns>true, if they are the same; otherwise, false.</returns>
        public static bool operator >(WinLoss? h1, WinLoss? h2)
        {
            return CompareToCommon(h1: h1, h2: h2) > 0;
        }

        /// <summary>
        ///     Equality comparison via operator overload
        /// </summary>
        /// <param name="h1">The first <see cref="WinLoss" />.</param>
        /// <param name="h2">The second <see cref="WinLoss" />.</param>
        /// <returns>true, if they are the same; otherwise, false.</returns>
        public static bool operator >=(WinLoss? h1, WinLoss? h2)
        {
            return CompareToCommon(h1: h1, h2: h2) >= 0;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is WinLoss x)
            {
                return AreEqual(this, h2: x);
            }

            return false;
        }

        /// <summary>
        ///     Adds two <see cref="WinLoss" /> values together.
        /// </summary>
        /// <param name="a">The first <see cref="WinLoss" />.</param>
        /// <param name="b">The second <see cref="WinLoss" />.</param>
        /// <returns>The result of the addition.</returns>
        public static WinLoss operator +(WinLoss a, WinLoss b)
        {
            return new(a.Value + b.Value);
        }

        /// <summary>
        ///     Subtracts the <paramref name="b" /> <see cref="WinLoss" /> value from <paramref name="a" />.
        /// </summary>
        /// <param name="a">The first <see cref="WinLoss" />.</param>
        /// <param name="b">The second <see cref="WinLoss" />.</param>
        /// <returns>The result of the subtraction.</returns>
        public static WinLoss operator -(WinLoss a, WinLoss b)
        {
            return new(a.Value - b.Value);
        }

        /// <summary>
        ///     Multiplies two <see cref="WinLoss" /> values.
        /// </summary>
        /// <param name="a">The first <see cref="WinLoss" />.</param>
        /// <param name="b">The second <see cref="WinLoss" />.</param>
        /// <returns>The two <see cref="WinLoss" /> multiplied.</returns>
        public static WinLoss operator *(WinLoss a, WinLoss b)
        {
            return new(a.Value * b.Value);
        }

        /// <summary>
        ///     Divides <paramref name="a" /> <see cref="WinLoss" /> value by <paramref name="b" />.
        /// </summary>
        /// <param name="a">The first <see cref="WinLoss" />.</param>
        /// <param name="b">The second <see cref="WinLoss" />.</param>
        /// <returns>The result of the division.</returns>
        public static WinLoss operator /(WinLoss a, WinLoss b)
        {
            return new(a.Value / b.Value);
        }

        private static bool AreEqual(WinLoss? h1, WinLoss? h2)
        {
            return ReferenceObjectHelpers.AreEqual(left: h1, right: h2, eq: (l, r) => l._value.Equals(r._value));
        }

        private static int CompareToCommon(WinLoss? h1, WinLoss? h2)
        {
            return ReferenceObjectHelpers.Compare(left: h1, right: h2, cmp: (l, r) => l._value.CompareTo(r._value));
        }

        /// <summary>Pads the value to a 32-byte boundary.</summary>
        private readonly struct LeftPadBytesAndFormattedTo32ByteBoundsStrategy : IHexIntegerBinaryPaddingStrategy
        {
            private const int PADDING_SIZE = 32;

            /// <summary>
            ///     Pads the bytes before they are formatted as a human readable string.
            /// </summary>
            /// <param name="value">The value to pad.</param>
            /// <returns>The padded value.</returns>
            public static byte[] PadFormattedValue(in byte[] value)
            {
                return value.LeftPad(padToBytes: PADDING_SIZE, fill: GetPaddingByte(value[0]));
            }

            /// <summary>
            ///     Pads the bytes before they are formatted as a binary value.
            /// </summary>
            /// <param name="value">The value to pad.</param>
            /// <returns>The padded value.</returns>
            public static byte[] PadBytes(in byte[] value)
            {
                return value.LeftPad(padToBytes: PADDING_SIZE, fill: GetPaddingByte(value[0]));
            }

            byte[] IHexIntegerBinaryPaddingStrategy.PadFormattedValue(in byte[] value)
            {
                return PadFormattedValue(in value);
            }

            byte[] IHexIntegerBinaryPaddingStrategy.PadBytes(in byte[] value)
            {
                return PadBytes(in value);
            }

            private static byte GetPaddingByte(byte b)
            {
                if ((b & 0x80) == 0x80)
                {
                    return 0xff;
                }

                return 0x00;
            }
        }
    }
}