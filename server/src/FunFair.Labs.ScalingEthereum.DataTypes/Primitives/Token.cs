using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using FunFair.Common.DataTypes.Helpers;
using FunFair.Common.DataTypes.Interfaces;
using FunFair.Common.Extensions;
using FunFair.Ethereum.DataTypes.Exceptions;
using FunFair.Ethereum.DataTypes.Interfaces;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Exceptions;

namespace FunFair.Labs.ScalingEthereum.DataTypes.Primitives
{
    /// <summary>
    ///     Represents generic tokens - must always be used when dealing with generic token!
    /// </summary>
    [DebuggerDisplay(value: "{ToString(), nq}  {Erc20Value}")]
    public sealed class Token : IEquatable<Token>, IComparable<Token>, IComparable, IConvertToBigEndianBytes, ITokenBalance<Token>
    {
        private const int DECIMAL_PLACES = 8;

        private const string SYMBOL = "SE667";

        /// <summary>
        ///     Size in Bytes.
        /// </summary>
        public const int RequiredBytesLength = 32;

        /// <summary>
        ///     Maximum length of the token when represented as a string.
        /// </summary>
        public const int MaxStringLength = RequiredBytesLength * 2 + 2;

        /// <summary>
        ///     The maximum string length.
        /// </summary>
        public const int MaximumStringLength = TokenAmount.MaximumStringLength;

        private readonly int _hashCode;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public Token()
            : this(BigInteger.Zero)
        {
        }

        /// <summary>
        ///     Constructs the <see cref="Token" /> from a hex string.
        /// </summary>
        /// <param name="value">The Hex <see cref="String" />.</param>
        public Token(string value)
            : this(FromString(value))
        {
        }

        /// <summary>
        ///     Constructs the <see cref="Token" /> from an ERC20 token.
        /// </summary>
        /// <param name="value">Value</param>
        public Token(Erc20TokenBalance value)
        {
            if (!StringComparer.CurrentCultureIgnoreCase.Equals(x: value.Symbol, y: SYMBOL) || value.DecimalPlaces != DECIMAL_PLACES)
            {
                throw new IncompatibleTokenException($"{value.Symbol} ({value.DecimalPlaces}) is not compatible with {SYMBOL} ({DECIMAL_PLACES})");
            }

            this.Erc20Value = value;
            this._hashCode = value.GetHashCode();
        }

        /// <summary>
        ///     Constructs the <see cref="Token" /> from a big integer.
        /// </summary>
        /// <param name="value">Value</param>
        public Token(BigInteger value)
            : this(ExtractErc20TokenBalance(value))
        {
        }

        /// <summary>
        ///     Constructs the <see cref="Token" /> from an integer.
        /// </summary>
        /// <param name="value">Value</param>
        public Token(long value)
            : this(new BigInteger(value))
        {
        }

        /// <summary>
        ///     Constructs the <see cref="Token" /> from a big integer.
        /// </summary>
        /// <param name="value">Value</param>
        public Token(decimal value)
            : this(Normalize(value))
        {
        }

        /// <summary>
        ///     <para>
        ///         Constructs the <see cref="Token" /> from a <see langword="byte" />
        ///     </para>
        ///     <para>array.</para>
        /// </summary>
        /// <param name="value">Value</param>
        public Token(byte[] value)
            : this(BytesToBigInteger(value))
        {
        }

        /// <summary>
        ///     No token.
        /// </summary>
        public static Token Zero { get; } = new(BigInteger.Zero);

        /// <summary>
        ///     Gets the value as an ERC20 token.
        /// </summary>
        public Erc20TokenBalance Erc20Value { get; }

        /// <inheritDoc />
        public int CompareTo(object? obj)
        {
            if (ReferenceEquals(objA: obj, objB: null))
            {
                return 1;
            }

            if (obj is Token token)
            {
                return CompareCommon(this, rhs: token);
            }

            throw new ArgumentException(message: "Should be a token object", nameof(obj));
        }

        /// <inheritDoc />
        public int CompareTo(Token? other)
        {
            if (ReferenceEquals(objA: other, objB: null))
            {
                return 1;
            }

            return CompareCommon(this, rhs: other);
        }

        /// <summary>
        ///     Extracts the value to a big Endian Byte Array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToBigEndianByteArray()
        {
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return this.Erc20Value.ToBigEndianByteArray();
        }

        /// <inheritDoc />
        public bool Equals(Token? other)
        {
            return EqualsCommon(this, f2: other);
        }

        /// <summary>
        ///     The Symbol to display.
        /// </summary>
        [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1822:MarkMembersAsStatic", Justification = "Makes sense here")]
        public string Symbol => SYMBOL;

        /// <summary>
        ///     The number of decimal places.
        /// </summary>
        [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1822:MarkMembersAsStatic", Justification = "Makes sense here")]
        public int DecimalPlaces => DECIMAL_PLACES;

        /// <summary>
        ///     Gets the value.
        /// </summary>
        public TokenAmount TokenAmount => this.Erc20Value.TokenAmount;

        private static Erc20TokenBalance ExtractErc20TokenBalance(BigInteger value)
        {
            try
            {
                return new Erc20TokenBalance(value: value, symbol: SYMBOL, decimalPlaces: DECIMAL_PLACES);
            }
            catch (NegativeAmountException exception)
            {
                throw new NegativeTokenException(exception);
            }
        }

        private static int CompareCommon(Token? lhs, Token? rhs)
        {
            if (ReferenceEquals(objA: lhs, objB: rhs))
            {
                return 0;
            }

            if (ReferenceEquals(objA: lhs, objB: null))
            {
                return int.MaxValue;
            }

            if (ReferenceEquals(objA: rhs, objB: null))
            {
                return int.MinValue;
            }

            return Comparer<BigInteger>.Default.Compare(x: lhs.Erc20Value.TokenAmount.Value, y: rhs.Erc20Value.TokenAmount.Value);
        }

        private static BigInteger Normalize(decimal value)
        {
            int decimalPlacesToUnit = DECIMAL_PLACES;

            string workingValue = RemoveTrailingZeros(value.ToString(CultureInfo.InvariantCulture));

            int decimalPlacesSource = GetDecimalPlaces(workingValue);

            workingValue = workingValue.Replace(oldValue: @".", newValue: string.Empty);

            while (decimalPlacesSource > 0 && decimalPlacesToUnit > 0)
            {
                --decimalPlacesSource;
                --decimalPlacesToUnit;
            }

            if (decimalPlacesSource == 0)
            {
                if (decimalPlacesToUnit > 0)
                {
                    workingValue += new string(c: '0', count: decimalPlacesToUnit);
                }
            }
            else
            {
                int removePoint = workingValue.Length - decimalPlacesSource;

                if (removePoint > 0)
                {
                    workingValue = workingValue.Substring(startIndex: 0, length: removePoint);
                }
            }

            return BigInteger.Parse(workingValue.TrimStart(trimChar: '0'), provider: CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Attempts to create a new <see cref="Token" /> object from the source text.
        /// </summary>
        /// <param name="source">The source value</param>
        /// <param name="result">The resulting value..</param>
        /// <returns>true, if the value could be converted; otherwise, false.</returns>
        public static bool TryParse(string? source, [NotNullWhen(returnValue: true)] out Token? result)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                result = default;

                return false;
            }

            try
            {
                result = new Token(source);

                return true;
            }
            catch
            {
                result = default;

                return false;
            }
        }

        /// <summary>
        ///     Transfers Token <paramref name="from" /> one object
        ///     <paramref name="to" /> another
        /// </summary>
        /// <param name="from">The source Token.</param>
        /// <param name="to">The destination Token</param>
        /// <param name="amount">
        ///     The integer amount of Token <paramref name="to" /> transfer
        /// </param>
        public static void Transfer(ref Token from, ref Token to, int amount)
        {
            Transfer(@from: ref from, to: ref to, new Token(amount));
        }

        /// <summary>
        ///     Transfers Token <paramref name="from" /> one object
        ///     <paramref name="to" /> another
        /// </summary>
        /// <param name="from">The source Token.</param>
        /// <param name="to">The destination Token</param>
        /// <param name="amount">
        ///     <para>
        ///         The <see cref="BigInteger" /> <paramref name="amount" /> of Token
        ///         <paramref name="to" />
        ///     </para>
        ///     <para>transfer</para>
        /// </param>
        public static void Transfer(ref Token from, ref Token to, BigInteger amount)
        {
            Transfer(@from: ref from, to: ref to, new Token(amount));
        }

        /// <summary>
        ///     Transfers Token <paramref name="from" /> one object
        ///     <paramref name="to" /> another
        /// </summary>
        /// <param name="from">The source Token.</param>
        /// <param name="to">The destination Token</param>
        /// <param name="amount">
        ///     The amount of Token <paramref name="to" /> transfer
        /// </param>
        public static void Transfer(ref Token from, ref Token to, Token amount)
        {
            Erc20TokenBalance fromToken = from.Erc20Value;
            Erc20TokenBalance toToken = to.Erc20Value;

            Erc20TokenBalance.Transfer(@from: ref fromToken, to: ref toToken, amount: amount.Erc20Value);

            from = new Token(fromToken);
            to = new Token(toToken);
        }

        /// <summary>
        ///     <para>
        ///         <see cref="Convert" /> the token value to a 32 <see langword="byte" />
        ///     </para>
        ///     <para>array (aways big endian), packed with zeros</para>
        /// </summary>
        /// <returns>
        ///     The array of bytes
        /// </returns>
        public byte[] ToBigEndianPackedByteArray()
        {
            return this.ToBigEndianByteArray()
                       .LeftPad(padToBytes: 32);
        }

        /// <inheritDoc />
        public override int GetHashCode()
        {
            return this._hashCode;
        }

        /// <inheritDoc />
        public override string ToString()
        {
            return HexEncodedString.Create(this.ToBigEndianByteArray());
        }

        /// <inheritDoc />
        public override bool Equals(object? obj)
        {
            if (obj is Token token)
            {
                return EqualsCommon(this, f2: token);
            }

            return false;
        }

        /// <summary>
        ///     checks that <paramref name="f1" /> is greater than <paramref name="f2" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool operator >(Token? f1, Token? f2)
        {
            return CompareCommon(lhs: f1, rhs: f2) > 0;
        }

        /// <summary>
        ///     checks that <paramref name="f1" /> is less than <paramref name="f2" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool operator <(Token? f1, Token? f2)
        {
            return CompareCommon(lhs: f1, rhs: f2) < 0;
        }

        /// <summary>
        ///     checks that <paramref name="f1" /> is greater than or equal to <paramref name="f2" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool operator >=(Token? f1, Token? f2)
        {
            return CompareCommon(lhs: f1, rhs: f2) >= 0;
        }

        /// <summary>
        ///     checks that <paramref name="f1" /> is les than or equal to <paramref name="f2" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool operator <=(Token? f1, Token? f2)
        {
            return CompareCommon(lhs: f1, rhs: f2) <= 0;
        }

        /// <summary>
        ///     checks that <paramref name="f1" /> is equal to <paramref name="f2" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool operator ==(Token? f1, Token? f2)
        {
            return EqualsCommon(f1: f1, f2: f2);
        }

        private static bool EqualsCommon(Token? f1, Token? f2)
        {
            if (ReferenceEquals(objA: f1, objB: f2))
            {
                return true;
            }

            if (ReferenceEquals(objA: f2, objB: null))
            {
                return false;
            }

            if (ReferenceEquals(objA: f1, objB: null))
            {
                return false;
            }

            return f1.Erc20Value == f2.Erc20Value;
        }

        /// <summary>
        ///     checks that <paramref name="f1" /> is not equal to <paramref name="f2" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool operator !=(Token? f1, Token? f2)
        {
            return !EqualsCommon(f1: f1, f2: f2);
        }

        /// <summary>
        ///     Adds <paramref name="f1" /> and <paramref name="f2" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static Token operator +(Token f1, Token f2)
        {
            return new(f1.Erc20Value + f2.Erc20Value);
        }

        /// <summary>
        ///     Adds <paramref name="f1" /> and <paramref name="f2" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static Token operator +(Token f1, BigInteger f2)
        {
            return EnsurePositive(action: () => new Token(f1.Erc20Value + f2));
        }

        /// <summary>
        ///     Subtracts <paramref name="f2" /> from <paramref name="f1" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static Token operator -(Token f1, Token f2)
        {
            return EnsurePositive(action: () => new Token(f1.Erc20Value - f2.Erc20Value));
        }

        /// <summary>
        ///     Subtracts <paramref name="f2" /> from <paramref name="f1" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static Token operator -(Token f1, BigInteger f2)
        {
            return EnsurePositive(action: () => new Token(f1.Erc20Value - f2));
        }

        /// <summary>
        ///     Multiplies <paramref name="f1" /> by <paramref name="f2" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static Token operator *(Token f1, Token f2)
        {
            return new(f1.Erc20Value * f2.Erc20Value);
        }

        /// <summary>
        ///     Multiplies <paramref name="f1" /> by <paramref name="f2" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static Token operator *(Token f1, BigInteger f2)
        {
            return EnsurePositive(action: () => new Token(f1.Erc20Value * f2));
        }

        /// <summary>
        ///     Multiplies <paramref name="f1" /> by <paramref name="i" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Token operator *(Token f1, int i)
        {
            return EnsurePositive(action: () => new Token(f1.Erc20Value * i));
        }

        /// <summary>
        ///     Divides <paramref name="f1" /> by <paramref name="f2" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static Token operator /(Token f1, Token f2)
        {
            return new(f1.Erc20Value / f2.Erc20Value);
        }

        /// <summary>
        ///     Divides <paramref name="f1" /> by <paramref name="f2" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static Token operator /(Token f1, BigInteger f2)
        {
            return EnsurePositive(action: () => new Token(f1.Erc20Value / f2));
        }

        /// <summary>
        ///     Divides <paramref name="f1" /> by <paramref name="i" />.
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Token operator /(Token f1, int i)
        {
            return EnsurePositive(action: () => new Token(f1.Erc20Value / i));
        }

        /// <summary>
        ///     Extracts the value from the string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NegativeTokenException"></exception>
        private static BigInteger FromString(string value)
        {
            BigInteger val = BigIntegerHelpers.FromUnsignedString(value);

            // negative BAN isn't possible, don't allow it to be created
            if (val < 0)
            {
                throw new NegativeTokenException(message: "BAN cannot be less than zero");
            }

            return val;
        }

        private static BigInteger BytesToBigInteger(in ReadOnlySpan<byte> bytes)
        {
            return bytes.FromBigEndianByteArrayToBigInteger();
        }

        private static int GetDecimalPlaces(string source)
        {
            int position = source.LastIndexOf(value: '.');

            if (position == -1)
            {
                return 0;
            }

            return source.Length - (position + 1);
        }

        private static string RemoveTrailingZeros(string source)
        {
            if (source.LastIndexOf(value: '.') == -1)
            {
                return source;
            }

            return source.TrimEnd(trimChar: '0')
                         .TrimEnd(trimChar: '.');
        }

        private static Token EnsurePositive(Func<Token> action)
        {
            try
            {
                return action();
            }
            catch (NegativeAmountException exception)
            {
                throw new NegativeTokenException(exception);
            }
        }
    }
}