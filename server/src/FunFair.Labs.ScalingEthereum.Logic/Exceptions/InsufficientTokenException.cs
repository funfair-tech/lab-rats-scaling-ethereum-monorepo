using System;

namespace FunFair.Labs.ScalingEthereum.Logic.Exceptions
{
    /// <summary>
    ///     Thrown when the faucet has not got sufficient token (or eth) to be able to issue any to the requestor.
    /// </summary>
    public sealed class InsufficientTokenException : Exception
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        public InsufficientTokenException()
            : this(message: "Faucet has insufficient fun available")
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">The message to return.</param>
        public InsufficientTokenException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="message">The message to return.</param>
        /// <param name="innerException">The inner exception.</param>
        public InsufficientTokenException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }
    }
}