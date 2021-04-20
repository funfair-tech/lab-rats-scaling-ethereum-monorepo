using FunFair.Common.Data;
using FunFair.Common.Data.Builders;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders.Models;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.DataManagers
{
    /// <summary>
    ///     Lock manager for Game Manager Contract.
    /// </summary>
    public sealed class GameManagerLockDataManager : ObjectLockDataManagerBase<EthereumAddress>
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="lockBuilder">Lock builder.</param>
        /// <param name="logger">Logging.</param>
        public GameManagerLockDataManager(ISqlServerDatabase database,
                                          IObjectBuilder<ObjectLockEntity<EthereumAddress>, ObjectLock<EthereumAddress>> lockBuilder,
                                          ILogger<GameManagerLockDataManager> logger)
            : base(database: database,
                   lockBuilder: lockBuilder,
                   new ObjectLockProcedures(clear: @"Locking.GameManager_Clear",
                                            acquire: @"Locking.GameManager_Acquire",
                                            release: @"Locking.GameManager_Release",
                                            isLocked: @"Locking.GameManager_IsLocked"),
                   logger: logger)
        {
        }
    }
}