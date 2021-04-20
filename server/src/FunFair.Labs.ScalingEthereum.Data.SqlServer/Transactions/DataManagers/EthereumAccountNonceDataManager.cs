using System;
using System.Threading.Tasks;
using FunFair.Common.Data;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Transactions.Data.Interfaces;
using FunFair.Ethereum.Transactions.Data.Interfaces.Exceptions;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.DataManagers
{
    /// <summary>
    ///     MS SQL Server binding of class which can manage nonces for Ethereum Accounts
    /// </summary>
    public sealed class EthereumAccountNonceDataManager : IEthereumAccountNonceDataManager
    {
        private readonly ISqlServerDatabase _database;
        private readonly string _machineName;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="database">The database to connect to.</param>
        public EthereumAccountNonceDataManager(ISqlServerDatabase database)
        {
            this._database = database ?? throw new ArgumentNullException(nameof(database));
            this._machineName = Environment.MachineName;
        }

        /// <inheritdoc />
        public async Task<EthereumAccountNonce> GetNextNonceAsync(INetworkAccount account)
        {
            AccountNonceEntity? accountNonceEntity = await this._database.QuerySingleOrDefaultAsync<object, AccountNonceEntity>(
                storedProcedure: @"Ethereum.AccountNonce_Get",
                new {Account = account.Address, Network = account.Network.Name, MachineName = this._machineName});

            if (accountNonceEntity?.Nonce == null)
            {
                throw new NonceNotAvailableForAccountException($"Could not retrieve nonce for {account.Address} on {account.Network.Name}");
            }

            return new EthereumAccountNonce(account: account, nonce: accountNonceEntity.Nonce);
        }

        /// <inheritdoc />
        public Task InitialiseAsync(EthereumAccountNonce nonce)
        {
            return this._database.ExecuteAsync(storedProcedure: @"Ethereum.AccountNonce_Initialise", new {Network = nonce.Account.Network.Name, Account = nonce.Account.Address, nonce.Nonce});
        }
    }
}