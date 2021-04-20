using System;
using System.Diagnostics.CodeAnalysis;

namespace FunFair.Labs.ScalingEthereum.Authentication.Exceptions
{
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Not all constructors are used")]
    public sealed class JwtUserNullException : Exception
    {
        public JwtUserNullException()
        {
        }

        public JwtUserNullException(string message)
            : base(message)
        {
        }

        public JwtUserNullException(string message, Exception innerException)
            : base(message: message, innerException: innerException)
        {
        }
    }
}