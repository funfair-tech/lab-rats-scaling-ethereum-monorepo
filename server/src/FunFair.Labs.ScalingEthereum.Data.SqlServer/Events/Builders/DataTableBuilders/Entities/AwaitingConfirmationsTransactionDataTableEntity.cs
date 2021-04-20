using System;
using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders.Entities
{
    /// <summary>
    ///     A 'risky' transaction entity.
    /// </summary>
    [DebuggerDisplay("{Network}: {TransactionHash} Contract: {ContractAddress} Event: {EventSignature} Index {EventIndex}")]
    public sealed record AwaitingConfirmationsTransactionDataTableEntity
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <param name="transactionHash">The transaction hash.</param>
        /// <param name="contractAddress">The contract address of the event.</param>
        /// <param name="eventSignature">The event signature of the event.</param>
        /// <param name="eventIndex">The index of the event within the transaction.</param>
        /// <param name="gasUsed">The amount of gas that was used.</param>
        /// <param name="gasPrice">The price paid for the gas that was used.</param>
        /// <param name="blockNumberFirstSeen">The block number the event was first seen in.</param>
        /// <param name="earliestBlockNumberForProcessing">The earliest block number the transaction can be processed in.</param>
        /// <param name="confirmationsToWaitFor">The number of confirmations that need to be waited for before the transaction can be considered final.</param>
        public AwaitingConfirmationsTransactionDataTableEntity(string network,
                                                               TransactionHash transactionHash,
                                                               ContractAddress contractAddress,
                                                               EventSignature eventSignature,
                                                               int eventIndex,
                                                               GasLimit? gasUsed,
                                                               GasPrice gasPrice,
                                                               BlockNumber blockNumberFirstSeen,
                                                               BlockNumber earliestBlockNumberForProcessing,
                                                               int confirmationsToWaitFor)
        {
            this.Network = network ?? throw new ArgumentNullException(nameof(network));
            this.TransactionHash = transactionHash ?? throw new ArgumentNullException(nameof(transactionHash));
            this.ContractAddress = contractAddress ?? throw new ArgumentNullException(nameof(contractAddress));
            this.EventSignature = eventSignature ?? throw new ArgumentNullException(nameof(eventSignature));
            this.EventIndex = eventIndex;
            this.GasUsed = gasUsed;
            this.GasPrice = gasPrice ?? throw new ArgumentNullException(nameof(gasPrice));
            this.BlockNumberFirstSeen = (int) (blockNumberFirstSeen ?? throw new ArgumentNullException(nameof(blockNumberFirstSeen))).Value;
            this.EarliestBlockNumberForProcessing = (int) (earliestBlockNumberForProcessing ?? throw new ArgumentNullException(nameof(earliestBlockNumberForProcessing))).Value;
            this.ConfirmationsToWaitFor = confirmationsToWaitFor;
        }

        /// <summary>The network the transaction is for.</summary>
        public string Network { get; }

        /// <summary>The hash of the transaction.</summary>
        public TransactionHash TransactionHash { get; }

        /// <summary>
        ///     The contract address.
        /// </summary>
        public ContractAddress ContractAddress { get; }

        /// <summary>
        ///     The event signature.
        /// </summary>
        public EventSignature EventSignature { get; }

        /// <summary>
        ///     The event index.
        /// </summary>
        public int EventIndex { get; }

        /// <summary>
        ///     The gas used by the transaction.
        /// </summary>
        public GasLimit? GasUsed { get; }

        /// <summary>
        ///     The gas price of the transaction.
        /// </summary>
        public GasPrice GasPrice { get; }

        /// <summary>The block number the transaction was first seen.</summary>
        public int BlockNumberFirstSeen { get; }

        /// <summary>
        ///     The earliest block to wait for confirmations for.
        /// </summary>
        public int EarliestBlockNumberForProcessing { get; }

        /// <summary>The number of confirmations to wait for.</summary>
        public int ConfirmationsToWaitFor { get; }
    }
}