using FunFair.Common.Data;
using FunFair.Common.Data.Builders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders.Models;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.DataManagers
{
    /// <summary>
    ///     Lock manager for Game rounds.
    /// </summary>
    public sealed class GameRoundLockDataManager : ObjectLockDataManagerBase<GameRoundId>
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="lockBuilder">Lock builder.</param>
        /// <param name="logger">Logging.</param>
        public GameRoundLockDataManager(ISqlServerDatabase database, IObjectBuilder<ObjectLockEntity<GameRoundId>, ObjectLock<GameRoundId>> lockBuilder, ILogger<GameRoundLockDataManager> logger)
            : base(database: database,
                   lockBuilder: lockBuilder,
                   new ObjectLockProcedures(clear: @"Locking.GameRound_Clear", acquire: @"Locking.GameRound_Acquire", release: @"Locking.GameRound_Release", isLocked: @"Locking.GameRound_IsLocked"),
                   logger: logger)
        {
        }
    }
}