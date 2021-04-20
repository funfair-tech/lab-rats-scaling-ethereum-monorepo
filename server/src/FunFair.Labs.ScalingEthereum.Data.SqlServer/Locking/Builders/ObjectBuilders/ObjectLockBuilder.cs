using FunFair.Common.Data.Builders;
using FunFair.Common.Data.Extensions;
using FunFair.Common.DataTypes.Interfaces;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders.Models;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders
{
    /// <summary>
    ///     Builder of <see cref="ObjectLock{TDataType}" /> from <see cref="ObjectLockEntity{TDataType}" />.
    /// </summary>
    /// <typeparam name="TDataType">The datatype of the lock</typeparam>
    public sealed class ObjectLockBuilder<TDataType> : IObjectBuilder<ObjectLockEntity<TDataType>, ObjectLock<TDataType>>
        where TDataType : class, IHexString<TDataType>
    {
        /// <inheritdoc />
        public ObjectLock<TDataType>? Build(ObjectLockEntity<TDataType>? source)
        {
            if (source == null)
            {
                return null;
            }

            return new ObjectLock<TDataType>(lockedAt: source.LockedAt, lockedBy: source.LockedBy ?? source.DataError(x => x.LockedBy), objectId: source.ObjectId ?? source.DataError(x => x.ObjectId));
        }
    }
}