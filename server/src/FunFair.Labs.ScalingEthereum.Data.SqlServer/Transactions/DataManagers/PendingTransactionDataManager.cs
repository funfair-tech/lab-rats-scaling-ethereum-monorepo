using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunFair.Common.Data;
using FunFair.Common.Data.Builders;
using FunFair.Common.Extensions;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Transactions.Data.Interfaces;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.DataTableBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.DataManagers
{
    /// <summary>
    ///     MS SQL Server binding of class which can retrieve a list of pending transactions.
    /// </summary>
    public sealed class PendingTransactionDataManager : IPendingTransactionDataManager
    {
        private readonly ISqlServerDatabase _database;
        private readonly ISqlDataTableBuilder<NetworkTransactionReceiptEntity> _networkTransactionReceiptDataTableBuilder;
        private readonly IObjectCollectionBuilder<PendingTransactionEntity, PendingTransaction> _pendingTransactionBuilder;
        private readonly ISqlDataTableBuilder<PendingTransaction> _pendingTransactionsDataTableBuilder;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="database">The database to connect to.</param>
        /// <param name="pendingTransactionBuilder">Builder of <see cref="PendingTransaction" /> objects.</param>
        /// <param name="networkTransactionReceiptDataTableBuilder">Builder of pending transactions data tables.</param>
        /// <param name="pendingTransactionsDataTableBuilder">Builder of state transactions data tables.</param>
        public PendingTransactionDataManager(ISqlServerDatabase database,
                                             IObjectCollectionBuilder<PendingTransactionEntity, PendingTransaction> pendingTransactionBuilder,
                                             ISqlDataTableBuilder<NetworkTransactionReceiptEntity> networkTransactionReceiptDataTableBuilder,
                                             ISqlDataTableBuilder<PendingTransaction> pendingTransactionsDataTableBuilder)
        {
            this._networkTransactionReceiptDataTableBuilder = networkTransactionReceiptDataTableBuilder ?? throw new ArgumentNullException(nameof(networkTransactionReceiptDataTableBuilder));
            this._pendingTransactionsDataTableBuilder = pendingTransactionsDataTableBuilder ?? throw new ArgumentNullException(nameof(pendingTransactionsDataTableBuilder));
            this._database = database ?? throw new ArgumentNullException(nameof(database));
            this._pendingTransactionBuilder = pendingTransactionBuilder ?? throw new ArgumentNullException(nameof(pendingTransactionBuilder));
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<PendingTransaction>> GetPendingTransactionsAsync(INetworkAccount account)
        {
            return this._database.QueryAsync(builder: this._pendingTransactionBuilder,
                                             storedProcedure: @"Ethereum.Transaction_GetPending",
                                             new {Network = account.Network.Name, AccountAddress = account.Address});
        }

        /// <inheritdoc />
        public Task SaveReceiptsAsync(IReadOnlyList<NetworkTransactionReceipt> receipts)
        {
            return this._database.ExecuteAsync(storedProcedure: @"Ethereum.Transaction_SaveReceipts", new {Receipts = this._networkTransactionReceiptDataTableBuilder.Build(receipts.Select(Convert))});
        }

        /// <inheritdoc />
        public Task MarkTransactionsAsStaleAsync(IReadOnlyList<PendingTransaction> unMinedTransactions)
        {
            return this._database.ExecuteAsync(storedProcedure: @"Ethereum.Transaction_MarkStale", new {Receipts = this._pendingTransactionsDataTableBuilder.Build(unMinedTransactions)});
        }

        /// <inheritdoc />
        public Task MarkTransactionAsFatallyBrokenAsync(BrokenTransaction transaction, TransactionStatus reason)
        {
            return this._database.ExecuteAsync(storedProcedure: @"Ethereum.Transaction_MarkAsBroken", new {Network = transaction.Network.Name, transaction.TransactionHash, Status = reason.GetName()});
        }

        private static NetworkTransactionReceiptEntity Convert(NetworkTransactionReceipt transactionReceipt)
        {
            return new(transactionHash: transactionReceipt.TransactionHash, blockHash: transactionReceipt.BlockHash, transactionIndex: transactionReceipt.TransactionIndex, blockNumber:
                       transactionReceipt.BlockNumber, gasUsed: transactionReceipt.GasUsed, transactionReceipt.Status.GetName());
        }
    }
}