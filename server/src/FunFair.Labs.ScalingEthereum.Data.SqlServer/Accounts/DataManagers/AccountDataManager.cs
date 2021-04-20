using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FunFair.Common.Data;
using FunFair.Common.Data.Builders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Accounts.Builders.ObjectBuilders.Entities;
using FunFair.Server.Ethereum.Accounts.Data.Interfaces;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Accounts.DataManagers
{
    /// <summary>
    ///     Data manager for managing accounts.
    /// </summary>
    public sealed class AccountDataManager : IAccountDataManager
    {
        private readonly IObjectCollectionBuilder<ConfiguredAccountEntity, ConfiguredAccount> _accountBuilder;
        private readonly ISqlServerDatabase _database;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="database">The database to connect to.</param>
        /// <param name="accountBuilder">Builder of <see cref="ConfiguredAccount" /> items.</param>
        public AccountDataManager(ISqlServerDatabase database, IObjectCollectionBuilder<ConfiguredAccountEntity, ConfiguredAccount> accountBuilder)
        {
            this._database = database ?? throw new ArgumentNullException(nameof(database));
            this._accountBuilder = accountBuilder ?? throw new ArgumentNullException(nameof(accountBuilder));
        }

        /// <inheritdoc />
        public Task InsertAsync(ConfiguredAccount configuredAccount)
        {
            var parameters = new
                             {
                                 Network = configuredAccount.Network.Name,
                                 configuredAccount.AccountAddress,
                                 configuredAccount.Wallet,
                                 configuredAccount.TokenFundingAccountAddress,
                                 configuredAccount.Unlock,
                                 configuredAccount.Version,
                                 configuredAccount.Enabled
                             };

            return this._database.ExecuteAsync(storedProcedure: @"Accounts.Account_Insert", param: parameters);
        }

        /// <inheritdoc />
        public Task UpdateAsync(ConfiguredAccount configuredAccount)
        {
            var parameters = new
                             {
                                 Network = configuredAccount.Network.Name,
                                 configuredAccount.AccountAddress,
                                 configuredAccount.Wallet,
                                 configuredAccount.TokenFundingAccountAddress,
                                 configuredAccount.Unlock,
                                 configuredAccount.Version,
                                 configuredAccount.Enabled
                             };

            return this._database.ExecuteAsync(storedProcedure: @"Accounts.Account_Update", param: parameters);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<ConfiguredAccount>> GetAllAsync()
        {
            return this._database.QueryAsync(builder: this._accountBuilder, storedProcedure: @"Accounts.Account_GetAll");
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<ConfiguredAccount>> GetEnabledAsync()
        {
            return this._database.QueryAsync(builder: this._accountBuilder, storedProcedure: @"Accounts.Account_GetEnabled");
        }
    }
}