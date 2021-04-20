using System;
using System.Diagnostics.CodeAnalysis;

namespace FunFair.Labs.ScalingEthereum.Logic.Exceptions
{
    /// <summary>
    ///     Thrown when the faucet is asked to issue token for a specific player too often.
    /// </summary>
    public sealed class TooFrequentTokenException : Exception
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public TooFrequentTokenException()
            : this(message: "Token has been requested too frequently.")
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">The message to return.</param>
        public TooFrequentTokenException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">The message to return.</param>
        /// <param name="innerException">The inner exception.</param>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public TooFrequentTokenException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }
    }
}