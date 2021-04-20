using System.Diagnostics;
using FunFair.Ethereum.Events.Data.Interfaces;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     A locked event.
    /// </summary>
    [DebuggerDisplay("Id: {" + nameof(EventTransactionId) + "}")]
    public sealed record EventLockEntity : IEventLock
    {
        /// <summary>
        ///     The Event transaction id that was locked.
        /// </summary>
        public long EventTransactionId { get; init; }
    }
}