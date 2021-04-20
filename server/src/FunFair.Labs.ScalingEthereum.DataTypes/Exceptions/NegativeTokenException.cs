using System;
using System.Diagnostics.CodeAnalysis;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.DataTypes.Exceptions
{
    /// <summary>
    ///     Thrown when attempting to create <see cref="Token" /> which would have a negative value.
    /// </summary>
    public sealed class NegativeTokenException : Exception
    {
        private const string MESSAGE = "Invalid token amount.  Cannot be negative.";

        /// <summary>
        ///     Constructor.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Standard Exception constructor")]
        public NegativeTokenException()
            : this(MESSAGE)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">The message to return.</param>
        public NegativeTokenException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">The message to return.</param>
        /// <param name="innerException">The inner exception.</param>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Standard Exception constructor")]
        public NegativeTokenException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public NegativeTokenException(Exception innerException)
            : base(message: MESSAGE, innerException: innerException)
        {
        }
    }
}