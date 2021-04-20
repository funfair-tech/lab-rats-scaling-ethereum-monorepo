using System;
using System.Diagnostics.CodeAnalysis;
using FunFair.Common.DataTypes;
using FunFair.Common.DataTypes.Interfaces;
using FunFair.Common.DataTypes.Strategies;
using FunFair.Ethereum.DataTypes.Primitives.Strategies;

namespace FunFair.Labs.ScalingEthereum.DataTypes.Primitives
{
    public sealed class WalletAppId : IHexString<WalletAppId>
    {
        /// <summary>
        ///     Length of the <see cref="WalletAppId" /> when represented as a byte array.
        /// </summary>
        public const uint RequiredByteLength = 32;

        /// <summary>
        ///     Length of the <see cref="WalletAppId" /> when represented as a string.
        /// </summary>
        public const uint RequiredStringLength = RequiredByteLength * 2 + 2;

        private readonly Lazy<string> _formatted;
        private readonly ReadOnlyMemoryHexStringValue<KeccakHashBoundedStringValidator, PrefixedLowerCaseHexStringFormattingStrategy> _value;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="hex">The hex string to create the string from.</param>
        public WalletAppId(string hex)
            : this(new ReadOnlyMemoryHexStringValue<KeccakHashBoundedStringValidator, PrefixedLowerCaseHexStringFormattingStrategy>(hex))
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="bytes">The bytes to create the string from.</param>
        public WalletAppId(byte[] bytes)
            : this(new ReadOnlyMemoryHexStringValue<KeccakHashBoundedStringValidator, PrefixedLowerCaseHexStringFormattingStrategy>(bytes))
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="bytes">The bytes to create the string from.</param>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public WalletAppId(in ReadOnlySpan<byte> bytes)
            : this(new ReadOnlyMemoryHexStringValue<KeccakHashBoundedStringValidator, PrefixedLowerCaseHexStringFormattingStrategy>(bytes))
        {
        }

        private WalletAppId(in ReadOnlyMemoryHexStringValue<KeccakHashBoundedStringValidator, PrefixedLowerCaseHexStringFormattingStrategy> value)
        {
            this._value = value;
            this._formatted = new Lazy<string>(valueFactory: () => this._value.ToString());
        }

        /// <inheritdoc />
        public bool Equals(WalletAppId? other)
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
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return this._value.ToSpan();
        }

        /// <inheritdoc />
        public ReadOnlyMemory<byte> ToMemory()
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return this._value.ToMemory();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this._formatted.Value;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this._value.GetHashCode();
        }

        /// <summary>
        ///     Equality comparison via operator overload
        /// </summary>
        /// <param name="h1">The first <see cref="WalletAppId" /></param>
        /// <param name="h2">The second <see cref="WalletAppId" /></param>
        /// <returns>true, if they are the same; otherwise, false.</returns>
        public static bool operator ==(WalletAppId? h1, WalletAppId? h2)
        {
            return AreEqual(h1: h1, h2: h2);
        }

        /// <summary>
        ///     Inequality comparison via operator overload.
        /// </summary>
        /// <param name="h1">The first <see cref="WalletAppId" /></param>
        /// <param name="h2">The second <see cref="WalletAppId" /></param>
        /// <returns>true, if they are different; otherwise, false.</returns>
        public static bool operator !=(WalletAppId? h1, WalletAppId? h2)
        {
            return !AreEqual(h1: h1, h2: h2);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (!(obj is WalletAppId other))
            {
                return false;
            }

            return AreEqual(this, h2: other);
        }

        private static bool AreEqual(WalletAppId? h1, WalletAppId? h2)
        {
            return ReferenceObjectHelpers.AreEqual(left: h1, right: h2, eq: (l, r) => l._value.Equals(r._value));
        }

        /// <summary>
        ///     Attempts to create an encrypted private key from the given string
        /// </summary>
        /// <param name="source">the string to parse.</param>
        /// <param name="value">The app id, if it could be parsed; otherwise, null.</param>
        /// <returns>True, if the address was valid; otherwise, false.</returns>
        public static bool TryParse(string source, [NotNullWhen(returnValue: true)] out WalletAppId? value)
        {
            if (ReadOnlyMemoryHexStringValue.TryParse(source: source, out ReadOnlyMemoryHexStringValue<KeccakHashBoundedStringValidator, PrefixedLowerCaseHexStringFormattingStrategy> parsed))
            {
                value = new WalletAppId(parsed);

                return true;
            }

            value = null;

            return false;
        }
    }
}