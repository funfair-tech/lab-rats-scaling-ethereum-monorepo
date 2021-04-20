using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FunFair.Common.Data;
using FunFair.Common.Data.Builders;
using FunFair.Common.Extensions;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Events.Data.Interfaces;
using FunFair.Ethereum.Events.Data.Interfaces.Models;
using FunFair.Ethereum.Events.Interfaces;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.DataManagers
{
    /// <summary>
    ///     Event Data management
    /// </summary>
    public sealed class EventDataManager : IEventDataManager
    {
        private readonly ISqlDataTableBuilder<ContractAddressEntity> _contractsDataTableBuilder;
        private readonly ISqlServerDatabase _database;
        private readonly IObjectCollectionBuilder<EventContractCheckpointEntity, EventContractCheckpoint> _eventCheckpointBuilder;
        private readonly ISqlDataTableBuilder<EventIndexEntity> _eventIndexBuilder;
        private readonly IObjectCollectionBuilder<AwaitingConfirmationsTransactionEntity, AwaitingConfirmationsTransaction> _eventRiskyTransactionsBuilder;
        private readonly ISqlDataTableBuilder<AwaitingConfirmationsTransactionDataTableEntity> _eventRiskyTransactionsDataTableBuilder;
        private readonly string _machineName;
        private readonly IObjectCollectionBuilder<EventTransactionHashEntity, TransactionHash> _transactionHashBuilder;
        private readonly ISqlDataTableBuilder<EventTransactionEntity> _transactionHashDataTableBuilder;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="database">The database to read from.</param>
        /// <param name="transactionHashDataTableBuilder">Transaction hash table builder.</param>
        /// <param name="contractsDataTableBuilder">Contract address table builder.</param>
        /// <param name="eventRiskyTransactionsDataTableBuilder">Event Risky Transactions Data Table Builder.</param>
        /// <param name="eventRiskyTransactionsBuilder">Builder of <see cref="AwaitingConfirmationsTransaction" /> from <see cref="AwaitingConfirmationsTransactionEntity" />.</param>
        /// <param name="eventIndexBuilder">An Event index builder</param>
        /// <param name="eventCheckpointBuilder">Builder of <see cref="EventContractCheckpoint" /> from <see cref="EventContractCheckpointEntity" />.</param>
        /// <param name="transactionHashBuilder">Builder of <see cref="TransactionHash" /> from, <see cref="EventTransactionHashEntity" /> objects.</param>
        public EventDataManager(ISqlServerDatabase database,
                                ISqlDataTableBuilder<EventTransactionEntity> transactionHashDataTableBuilder,
                                ISqlDataTableBuilder<ContractAddressEntity> contractsDataTableBuilder,
                                ISqlDataTableBuilder<AwaitingConfirmationsTransactionDataTableEntity> eventRiskyTransactionsDataTableBuilder,
                                IObjectCollectionBuilder<AwaitingConfirmationsTransactionEntity, AwaitingConfirmationsTransaction> eventRiskyTransactionsBuilder,
                                ISqlDataTableBuilder<EventIndexEntity> eventIndexBuilder,
                                IObjectCollectionBuilder<EventContractCheckpointEntity, EventContractCheckpoint> eventCheckpointBuilder,
                                IObjectCollectionBuilder<EventTransactionHashEntity, TransactionHash> transactionHashBuilder)
        {
            this._database = database ?? throw new ArgumentNullException(nameof(database));
            this._transactionHashDataTableBuilder = transactionHashDataTableBuilder ?? throw new ArgumentNullException(nameof(transactionHashDataTableBuilder));
            this._contractsDataTableBuilder = contractsDataTableBuilder ?? throw new ArgumentNullException(nameof(contractsDataTableBuilder));
            this._eventRiskyTransactionsDataTableBuilder = eventRiskyTransactionsDataTableBuilder ?? throw new ArgumentNullException(nameof(eventRiskyTransactionsDataTableBuilder));
            this._eventRiskyTransactionsBuilder = eventRiskyTransactionsBuilder ?? throw new ArgumentNullException(nameof(eventRiskyTransactionsBuilder));
            this._eventIndexBuilder = eventIndexBuilder ?? throw new ArgumentNullException(nameof(eventIndexBuilder));
            this._eventCheckpointBuilder = eventCheckpointBuilder ?? throw new ArgumentNullException(nameof(eventCheckpointBuilder));
            this._transactionHashBuilder = transactionHashBuilder ?? throw new ArgumentNullException(nameof(transactionHashBuilder));
            this._machineName = Environment.MachineName;
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<TransactionHash>> GetUnprocessedAsync(EthereumNetwork network, IReadOnlyList<TransactionHash> transactionHashes)
        {
            var parameters = new {Network = network.Name, Transactions = this._transactionHashDataTableBuilder.Build(transactionHashes.Select(Convert))};

            return this._database.QueryAsync(builder: this._transactionHashBuilder, storedProcedure: @"Ethereum.EventData_GetUnProcessedTransactions", param: parameters);
        }

        /// <inheritdoc />
        public Task RecordFilterCurrentBlockAsync(EthereumNetwork network, IReadOnlyList<ContractAddress> addresses, BlockNumber currentBlock)
        {
            var parameters = new {Network = network.Name, Contracts = this.BuildContractList(addresses), BlockNumber = (int) currentBlock.Value, MachineName = this._machineName};

            return this._database.ExecuteAsync(storedProcedure: @"Ethereum.EventBlock_SetCurrentBlock", param: parameters);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<EventContractCheckpoint>> GetLatestProcessedBlockAsync(EthereumNetwork network, IReadOnlyList<ContractAddress> addresses)
        {
            var parameters = new {Network = network.Name, Contracts = this.BuildContractList(addresses), MachineName = this._machineName};

            return this._database.QueryAsync(builder: this._eventCheckpointBuilder, storedProcedure: @"Ethereum.EventBlock_GetLatestBlocks", param: parameters);
        }

        /// <inheritdoc />
        public Task RecordHighRiskTransactionsAsync(IReadOnlyList<AwaitingConfirmationsTransaction> highRisk)
        {
            var parameters = new {Transactions = this._eventRiskyTransactionsDataTableBuilder.Build(highRisk.Select(Convert))};

            return this._database.ExecuteAsync(storedProcedure: @"Ethereum.EventRiskyTransactions_Save", param: parameters);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<AwaitingConfirmationsTransaction>> GetHighRiskTransactionsReadyForProcessingAsync(EthereumNetwork network, BlockNumber currentBlock)
        {
            var parameters = new {Network = network.Name, BlockNumber = (int) currentBlock.Value};

            return this._database.QueryAsync(builder: this._eventRiskyTransactionsBuilder, storedProcedure: @"Ethereum.EventRiskyTransaction_GetReadyForProcessing", param: parameters);
        }

        /// <inheritdoc />
        public Task IgnoreHighRiskTransactionThatHasDisappearedAsync(EthereumNetwork network, TransactionHash transactionHash)
        {
            var parameters = new {Network = network.Name, TransactionHash = transactionHash};

            return this._database.ExecuteAsync(storedProcedure: @"Ethereum.EventRiskyTransaction_Ignore", param: parameters);
        }

        /// <inheritdoc />
        public async Task<IEventLock?> LockEventForProcessingAsync(EthereumNetwork network,
                                                                   ContractAddress contractAddresses,
                                                                   EventSignature eventSignature,
                                                                   TransactionHash transactionHash,
                                                                   int eventIndex,
                                                                   BlockNumber blockNumber,
                                                                   GasLimit gasUsed,
                                                                   GasPrice gasPrice,
                                                                   EventRetrievalStrategy retrievalStrategy)
        {
            var param = new
                        {
                            Network = network.Name,
                            ContractAddress = contractAddresses,
                            EventSignature = eventSignature,
                            TransactionHash = transactionHash,
                            EventIndex = eventIndex,
                            MachineName = this._machineName,
                            BlockNumber = (int) blockNumber.Value,
                            GasUsed = gasUsed,
                            GasPrice = gasPrice,
                            Strategy = retrievalStrategy.GetName()
                        };

            return await this._database.QuerySingleOrDefaultAsync<object, EventLockEntity>(storedProcedure: @"Ethereum.Event_Lock", param: param);
        }

        /// <inheritdoc />
        public Task ReleaseAsync(IEventLock context, bool completed)
        {
            EventLockEntity e = (EventLockEntity) context;

            var param = new {e.EventTransactionId, Completed = completed};

            return this._database.ExecuteAsync(storedProcedure: @"Ethereum.Event_Release", param: param);
        }

        /// <inheritdoc />
        public Task ExcludeChangedLogIndexesAsync(EthereumNetwork network, TransactionHash transactionHash, EventSignatureIndex[] eventIndexes)
        {
            var param = new {Network = network.Name, TransactionHash = transactionHash, EventIndexes = this._eventIndexBuilder.Build(eventIndexes.Select(Convert))};

            return this._database.ExecuteAsync(storedProcedure: @"Ethereum.Event_ExcludeChangedLogIndexes", param: param);
        }

        /// <inheritdoc />
        public Task ClearAllLocksAsync()
        {
            var param = new {MachineName = this._machineName};

            return this._database.ExecuteAsync(storedProcedure: @"Ethereum.Event_ClearLock", param: param);
        }

        private static EventTransactionEntity Convert(TransactionHash transactionHash)
        {
            return new(transactionHash);
        }

        private static EventIndexEntity Convert(EventSignatureIndex eventSignatureIndex)
        {
            return new(eventSignature: eventSignatureIndex.EventSignature, contractAddress: eventSignatureIndex.ContractAddress, index: eventSignatureIndex.Ordinal);
        }

        private SqlMapper.ICustomQueryParameter BuildContractList(IReadOnlyList<ContractAddress> addresses)
        {
            return this._contractsDataTableBuilder.Build(addresses.Select(Convert));
        }

        private static ContractAddressEntity Convert(ContractAddress contractAddress)
        {
            return new(contractAddress);
        }

        private static AwaitingConfirmationsTransactionDataTableEntity Convert(AwaitingConfirmationsTransaction transaction)
        {
            return new(network: transaction.Network.Name, contractAddress: transaction.ContractAddress, eventSignature: transaction.EventSignature, eventIndex: transaction.EventIndex, transactionHash:
                       transaction.TransactionHash, blockNumberFirstSeen: transaction.BlockNumberFirstSeen, earliestBlockNumberForProcessing:
                       transaction.BlockNumberFirstSeen + transaction.ConfirmationsToWaitFor, confirmationsToWaitFor: transaction.ConfirmationsToWaitFor, gasUsed: transaction.GasUsed, gasPrice:
                       transaction.GasPrice);
        }
    }
}