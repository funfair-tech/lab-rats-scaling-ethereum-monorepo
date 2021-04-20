using System;
using System.Threading.Tasks;
using FunFair.Common.Data;
using FunFair.Common.Data.Builders;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Transactions.Data.Interfaces;
using FunFair.Ethereum.Transactions.Data.Interfaces.Models;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.DataManagers
{
    /// <summary>
    ///     Data manager for getting transaction submission status summary.
    /// </summary>
    public sealed class TransactionStatusDataManager : ITransactionStatusDataManager
    {
        private readonly ISqlServerDatabase _database;
        private readonly IObjectBuilder<TransactionSubmissionStatusEntity, TransactionSubmissionStatus> _transactionSubmissionStatusBuilder;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="database">The database to connect to.</param>
        /// <param name="transactionSubmissionStatusBuilder">Builder of <see cref="TransactionSubmissionStatus" /> objects.</param>
        public TransactionStatusDataManager(ISqlServerDatabase database, IObjectCollectionBuilder<TransactionSubmissionStatusEntity, TransactionSubmissionStatus> transactionSubmissionStatusBuilder)
        {
            this._transactionSubmissionStatusBuilder = transactionSubmissionStatusBuilder ?? throw new ArgumentNullException(nameof(transactionSubmissionStatusBuilder));
            this._database = database ?? throw new ArgumentNullException(nameof(database));
        }

        /// <inheritdoc />
        public Task<TransactionSubmissionStatus?> GetStatusAsync(INetworkAccount networkAccount)
        {
            return this._database.QuerySingleOrDefaultAsync(builder: this._transactionSubmissionStatusBuilder,
                                                            storedProcedure: @"Ethereum.TransactionStatus_Get",
                                                            new {network = networkAccount.Network.Name, accountAddress = networkAccount.Address});
        }
    }
}