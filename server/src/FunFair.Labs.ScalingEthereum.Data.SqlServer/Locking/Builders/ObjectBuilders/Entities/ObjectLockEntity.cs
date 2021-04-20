using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FunFair.Common.DataTypes.Interfaces;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     Lock entity.
    /// </summary>
    [DebuggerDisplay("{ObjectId} Locked by {LockedBy} at {LockedAt}")]
    public sealed record ObjectLockEntity<TDataType>
        where TDataType : class, IHexString<TDataType>
    {
        /// <summary>
        ///     Fate Channel Id.
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Initialised by Dapper")]
        public TDataType? ObjectId { get; init; }

        /// <summary>
        ///     Which machine locked the fate channel.
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Initialised by Dapper")]
        public string? LockedBy { get; init; }

        /// <summary>
        ///     When the lock was taken.
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Initialised by Dapper")]
        public DateTime LockedAt { get; init; }
    }
}