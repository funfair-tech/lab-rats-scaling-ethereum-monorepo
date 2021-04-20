using System;
using System.Diagnostics.CodeAnalysis;
using FunFair.Common.DataTypes;
using FunFair.Common.DataTypes.Interfaces;
using FunFair.Common.DataTypes.Strategies;
using FunFair.Ethereum.DataTypes.Primitives.Strategies;

namespace FunFair.Labs.ScalingEthereum.DataTypes.Primitives
{
    /// <summary>
    ///     A Seed.
    /// </summary>
    public sealed class Seed : IHexString<Seed>
    {
        /// <summary>
        ///     Length of the <see cref="Seed" /> when represented as a byte array.
        /// </summary>
        public const uint RequiredByteLength = KeccakHashBoundedStringValidator.RequiredByteLength;

        /// <summary>
        ///     Length of the <see cref="Seed" /> when represented as a string.
        /// </summary>
        public const uint RequiredStringLength = RequiredByteLength * 2 + 2;

        private readonly ReadOnlyMemoryHexStringValue<KeccakHashBoundedStringValidator, PrefixedLowerCaseHexStringFormattingStrategy> _value;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="hex">The hex string to create the string from.</param>
        public Seed(string hex)
            : this(new ReadOnlyMemoryHexStringValue<KeccakHashBoundedStringValidator, PrefixedLowerCaseHexStringFormattingStrategy>(hex))
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="bytes">The bytes to create the string from.</param>
        public Seed(in byte[] bytes)
            : this(new ReadOnlyMemoryHexStringValue<KeccakHashBoundedStringValidator, PrefixedLowerCaseHexStringFormattingStrategy>(bytes))
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="bytes">The bytes to create the string from.</param>
        public Seed(in ReadOnlySpan<byte> bytes)
            : this(new ReadOnlyMemoryHexStringValue<KeccakHashBoundedStringValidator, PrefixedLowerCaseHexStringFormattingStrategy>(bytes))
        {
        }

        private Seed(in ReadOnlyMemoryHexStringValue<KeccakHashBoundedStringValidator, PrefixedLowerCaseHexStringFormattingStrategy> value)
        {
            this._value = value;
        }

        private static Seed None { get; } = new(new byte [RequiredByteLength]);

        /// <inheritdoc />
        public bool Equals(Seed? other)
        {
            return AreEqual(this, h2: other);
        }

        /// <inheritdoc />
        public uint ByteLength => this._value.ByteLength;

        /// <inheritdoc />
        public uint StringLength => this._value.StringLength;

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

        /// <summary>
        ///     Attempts to create a <see cref="Seed" /> from the given string
        /// </summary>
        /// <param name="source">the string to parse.</param>
        /// <param name="value">The <see cref="Seed" />, if it could be parsed; otherwise, null.</param>
        /// <returns>True, if <see cref="Seed" /> was valid; otherwise, false.</returns>
        public static bool TryParse(string? source, [NotNullWhen(returnValue: true)] out Seed? value)
        {
            if (ReadOnlyMemoryHexStringValue.TryParse(source: source, out ReadOnlyMemoryHexStringValue<KeccakHashBoundedStringValidator, PrefixedLowerCaseHexStringFormattingStrategy> parsed))
            {
                if (None.ToSpan()
                        .SequenceEqual(parsed.ToSpan()))
                {
                    value = None;

                    return true;
                }

                value = new Seed(parsed);

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
        /// <param name="h1">The first <see cref="Seed" />.</param>
        /// <param name="h2">The second <see cref="Seed" />.</param>
        /// <returns>true, if they are the same; otherwise, false.</returns>
        public static bool operator ==(Seed? h1, Seed? h2)
        {
            return AreEqual(h1: h1, h2: h2);
        }

        /// <summary>
        ///     Inequality comparison via operator overload.
        /// </summary>
        /// <param name="h1">The first <see cref="Seed" />.</param>
        /// <param name="h2">The second <see cref="Seed" />.</param>
        /// <returns>true, if they are different; otherwise, false.</returns>
        public static bool operator !=(Seed? h1, Seed? h2)
        {
            return !AreEqual(h1: h1, h2: h2);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (!(obj is Seed other))
            {
                return false;
            }

            return AreEqual(this, h2: other);
        }

        private static bool AreEqual(Seed? h1, Seed? h2)
        {
            return ReferenceObjectHelpers.AreEqual(left: h1, right: h2, eq: (l, r) => l._value.Equals(r._value));
        }
    }
}