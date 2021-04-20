using System;
using System.Net;
using System.Threading.Tasks;
using FunFair.Common.Data;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.Faucet;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Faucet.DataManagers
{
    /// <summary>
    ///     Data manager for faucet access.
    /// </summary>
    public sealed class FaucetDataManager : IFaucetDataManager
    {
        private readonly ISqlServerDatabase _database;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="database">The database connection.</param>
        public FaucetDataManager(ISqlServerDatabase database)
        {
            this._database = database ?? throw new ArgumentNullException(nameof(database));
        }

        /// <inheritdoc />
        public Task<bool> IsAllowedToIssueFromFaucetAsync(IPAddress ipAddress, AccountAddress address)
        {
            return this._database.QuerySingleOrDefaultValueAsync<object, bool>(storedProcedure: @"Faucet.Minting_Allow", new {IPAddress = ipAddress, Address = address});
        }
    }
}