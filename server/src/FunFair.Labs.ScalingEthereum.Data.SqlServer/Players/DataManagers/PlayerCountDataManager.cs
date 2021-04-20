using System;
using System.Threading.Tasks;
using FunFair.Common.Data;
using FunFair.Ethereum.DataTypes;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.PlayerCount;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Players.DataManagers
{
    /// <inheritdoc />
    /// <summary>
    ///     Player count data manager
    /// </summary>
    public sealed class PlayerCountDataManager : IPlayerCountDataManager
    {
        private readonly ISqlServerDatabase _database;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="database">The database to connect to.</param>
        public PlayerCountDataManager(ISqlServerDatabase database)
        {
            this._database = database ?? throw new ArgumentNullException(nameof(database));
        }

        /// <inheritdoc />
        public Task<int> GetAsync(EthereumNetwork network)
        {
            return this._database.QuerySingleAsync<object, int>(storedProcedure: @"Players.PlayerCount_Get", new {Network = network.Name});
        }

        /// <inheritdoc />
        public Task ResetAsync(string machineName)
        {
            return this._database.ExecuteAsync(storedProcedure: @"Players.PlayerCount_Reset", new {MachineName = machineName});
        }

        /// <inheritdoc />
        public Task<int> IncrementAsync(string machineName, EthereumNetwork network)
        {
            return this._database.QuerySingleAsync<object, int>(storedProcedure: @"Players.PlayerCount_Increment", new {machineName, Network = network.Name});
        }

        /// <inheritdoc />
        public Task<int> DecrementAsync(string machineName, EthereumNetwork network)
        {
            return this._database.QuerySingleAsync<object, int>(storedProcedure: @"Players.PlayerCount_Decrement", new {machineName, Network = network.Name});
        }
    }
}