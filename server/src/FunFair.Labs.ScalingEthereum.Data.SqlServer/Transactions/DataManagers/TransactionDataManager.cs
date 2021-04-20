using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunFair.Common.Data;
using FunFair.Common.Data.Builders;
using FunFair.Common.Extensions;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Transactions.Data.Interfaces;
using FunFair.Ethereum.Transactions.Data.Interfaces.Models;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.DataManagers
{
    /// <summary>
    ///     MS SQL Server binding of class which can save transactions.
    /// </summary>
    public sealed class TransactionDataManager : ITransactionDataManager
    {
        private readonly IObjectCollectionBuilder<BrokenTransactionEntity, BrokenTransaction> _brokenTransactionBuilder;
        private readonly ISqlServerDatabase _database;
        private readonly IObjectBuilder<PendingTransactionEntity, PendingTransaction> _pendingTransactionBuilder;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="database">The database to connect to.</param>
        /// <param name="brokenTransactionBuilder">Builder of <see cref="BrokenTransaction" /> objects.</param>
        /// <param name="pendingTransactionBuilder">Builder of <see cref="PendingTransaction" /> objects.</param>
        public TransactionDataManager(ISqlServerDatabase database,
                                      IObjectCollectionBuilder<BrokenTransactionEntity, BrokenTransaction> brokenTransactionBuilder,
                                      IObjectBuilder<PendingTransactionEntity, PendingTransaction> pendingTransactionBuilder)
        {
            this._database = database ?? throw new ArgumentNullException(nameof(database));
            this._brokenTransactionBuilder = brokenTransactionBuilder ?? throw new ArgumentNullException(nameof(brokenTransactionBuilder));
            this._pendingTransactionBuilder = pendingTransactionBuilder ?? throw new ArgumentNullException(nameof(pendingTransactionBuilder));
        }

        /// <inheritdoc />
        public Task SaveNewAsync(INetworkAccount account, NewTransaction transaction, TransactionContext? context)
        {
            var parameters = new
                             {
                                 Network = account.Network.Name,
                                 Account = account.Address,
                                 transaction.ContractAddress,
                                 transaction.FunctionName,
                                 TransactionData = transaction.Data,
                                 transaction.TransactionHash,
                                 transaction.Value,
                                 transaction.GasPrice,
                                 transaction.GasLimit,
                                 transaction.EstimatedGas,
                                 transaction.Nonce,
                                 context?.ContextType,
                                 context?.ContextId,
                                 Priority = transaction.Priority.GetName()
                             };

            return this._database.ExecuteAsync(storedProcedure: @"Ethereum.Transaction_Insert", param: parameters);
        }

        /// <inheritdoc />
        public Task SaveReplacementAsync(INetworkAccount account,
                                         NewTransaction transaction,
                                         TransactionHash replacesTransactionHash,
                                         TransactionReplacementReason reasonForTransactionReplacement,
                                         TransactionState chainStatusOfPreviousTransaction)
        {
            var parameters = new
                             {
                                 Network = account.Network.Name,
                                 Account = account.Address,
                                 transaction.ContractAddress,
                                 transaction.FunctionName,
                                 TransactionData = transaction.Data,
                                 transaction.TransactionHash,
                                 transaction.Value,
                                 transaction.GasPrice,
                                 transaction.GasLimit,
                                 transaction.EstimatedGas,
                                 transaction.Nonce,
                                 ReplacesTransactionHash = replacesTransactionHash,
                                 ReasonForSubmission = reasonForTransactionReplacement.GetName(),
                                 ChainStatusOfPreviousTransaction = chainStatusOfPreviousTransaction.GetName(),
                                 Priority = transaction.Priority.GetName()
                             };

            return this._database.ExecuteAsync(storedProcedure: @"Ethereum.Transaction_InsertReplacement", param: parameters);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<BrokenTransaction>> GetBrokenTransactionsAsync(INetworkAccount account, DateTime currentNetworkDateTime)
        {
            var parameters = new {Network = account.Network.Name, Account = account.Address, CurrentTimeOnNetwork = currentNetworkDateTime};

            IReadOnlyList<BrokenTransaction> entities = await this._database.QueryAsync(builder: this._brokenTransactionBuilder, storedProcedure: @"Ethereum.Transaction_GetBroken", param: parameters);

            return entities.OrderBy(keySelector: x => x.Nonce)
                           .ToList();
        }

        /// <inheritdoc />
        public Task<PendingTransaction?> GetTransactionByHashAsync(INetworkAccount account, TransactionHash transactionHash)
        {
            var parameters = new {Network = account.Network.Name, Account = account.Address, TransactionHash = transactionHash};

            return this._database.QuerySingleOrDefaultAsync(builder: this._pendingTransactionBuilder, storedProcedure: @"Ethereum.Transaction_GetByTransactionHash", param: parameters);
        }

        /// <inheritdoc />
        public Task MarkAsCannotSpeedUpAsync(EthereumNetwork network, TransactionHash transactionHash)
        {
            var parameters = new {Network = network.Name, TransactionHash = transactionHash};

            return this._database.ExecuteAsync(storedProcedure: @"Ethereum.Transaction_MarkAsCannotSpeedUp", param: parameters);
        }
    }
}