using System;
using System.Diagnostics;
using FunFair.Common.DataTypes.Interfaces;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders.Models
{
    /// <summary>
    ///     An object lock.
    /// </summary>
    /// <typeparam name="TDataType"></typeparam>
    [DebuggerDisplay("{ObjectId} Locked by {LockedBy} at {LockedAt}")]
    public sealed class ObjectLock<TDataType>
        where TDataType : class, IHexString<TDataType>
    {
        /// <summary>
        ///     Consrtructor.
        /// </summary>
        /// <param name="objectId">The object id.</param>
        /// <param name="lockedBy">The machine that locked the object.</param>
        /// <param name="lockedAt">When the lock was taken.</param>
        public ObjectLock(TDataType objectId, string lockedBy, DateTime lockedAt)
        {
            this.ObjectId = objectId;
            this.LockedBy = lockedBy;
            this.LockedAt = lockedAt;
        }

        /// <summary>
        ///     Object Id.
        /// </summary>
        public TDataType ObjectId { get; }

        /// <summary>
        ///     Which machine locked the object.
        /// </summary>
        public string LockedBy { get; }

        /// <summary>
        ///     When the lock was taken.
        /// </summary>
        public DateTime LockedAt { get; }
    }
}