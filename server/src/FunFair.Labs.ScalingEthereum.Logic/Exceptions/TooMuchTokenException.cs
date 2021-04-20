using System;
using System.Diagnostics.CodeAnalysis;

namespace FunFair.Labs.ScalingEthereum.Logic.Exceptions
{
    /// <summary>
    ///     Thrown when the requester has too much token to be issued any by the faucet.
    /// </summary>
    public sealed class TooMuchTokenException : Exception
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public TooMuchTokenException()
            : this(message: "You have too much token to request more at this time.")
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">The message to return.</param>
        public TooMuchTokenException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">The message to return.</param>
        /// <param name="innerException">The inner exception.</param>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Standard exception constructor")]
        public TooMuchTokenException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }
    }
}