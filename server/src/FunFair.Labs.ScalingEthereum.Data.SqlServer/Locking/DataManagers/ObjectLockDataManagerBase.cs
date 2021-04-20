using System;
using System.Threading.Tasks;
using FunFair.Common.Data;
using FunFair.Common.Data.Builders;
using FunFair.Common.DataTypes.Interfaces;
using FunFair.Common.ObjectLocking.Data.Interfaces;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders.Models;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.DataManagers
{
    /// <summary>
    ///     Data manager for object locks.
    /// </summary>
    public abstract class ObjectLockDataManagerBase<TDataType> : IObjectLockDataManager<TDataType>
        where TDataType : class, IHexString<TDataType>
    {
        private readonly ISqlServerDatabase _database;
        private readonly IObjectBuilder<ObjectLockEntity<TDataType>, ObjectLock<TDataType>> _lockBuilder;
        private readonly ILogger _logger;
        private readonly string _machineName;
        private readonly ObjectLockProcedures _objectLockProcedures;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="lockBuilder">Lock builder.</param>
        /// <param name="objectLockProcedures">The procedures that should be used for accessing the Database locks.</param>
        /// <param name="logger">Logging.</param>
        protected ObjectLockDataManagerBase(ISqlServerDatabase database,
                                            IObjectBuilder<ObjectLockEntity<TDataType>, ObjectLock<TDataType>> lockBuilder,
                                            ObjectLockProcedures objectLockProcedures,
                                            ILogger logger)
        {
            this._database = database ?? throw new ArgumentNullException(nameof(database));
            this._lockBuilder = lockBuilder ?? throw new ArgumentNullException(nameof(lockBuilder));
            this._objectLockProcedures = objectLockProcedures ?? throw new ArgumentNullException(nameof(objectLockProcedures));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._machineName = Environment.MachineName;
        }

        /// <inheritdoc />
        public Task ClearAllLocksAsync()
        {
            return this._database.ExecuteAsync(storedProcedure: this._objectLockProcedures.Clear, new {MachineName = this._machineName});
        }

        /// <inheritdoc />
        public Task ReleaseAsync(TDataType objectId)
        {
            return this._database.ExecuteAsync(storedProcedure: this._objectLockProcedures.Release, new {ObjectId = objectId});
        }

        /// <inheritdoc />
        public async Task<bool> AcquireAsync(TDataType objectId)
        {
            ObjectLock<TDataType>? exists = await this._database.QuerySingleOrDefaultAsync(builder: this._lockBuilder,
                                                                                           storedProcedure: this._objectLockProcedures.Acquire,
                                                                                           new {ObjectId = objectId, MachineName = this._machineName});

            if (exists != null)
            {
                this._logger.LogDebug($"Object was locked by {exists.LockedBy} on {exists.LockedAt}");

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public async Task<bool> LockedAsync(TDataType objectId)
        {
            ObjectLock<TDataType>? exists = await this._database.QuerySingleOrDefaultAsync(builder: this._lockBuilder, storedProcedure: this._objectLockProcedures.IsLocked, new {ObjectId = objectId});

            if (exists != null)
            {
                this._logger.LogDebug($"Object is currently locked by {exists.LockedBy} on {exists.LockedAt}");

                return true;
            }

            return false;
        }
    }
}