using System.Diagnostics;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Models
{
    /// <summary>
    ///     The Pong Response
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public sealed class PongDto
    {
        /// <summary>
        ///     The value of the pong response.
        /// </summary>
        public string Value { get; init; } = default!;
    }
}