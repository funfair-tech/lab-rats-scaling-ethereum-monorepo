using System;
using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.DataTableBuilders.Entities
{
    /// <summary>
    ///     A Network Transaction Receipt.
    /// </summary>
    [DebuggerDisplay(value: "Txn: {TransactionHash} Block: {BlockHash} Status: {Status}")]
    public sealed record NetworkTransactionReceiptEntity
    {
        /// <summary>Constructor.</summary>
        /// <param name="transactionHash">The transaction hash.</param>
        /// <param name="blockHash">The block hash the transaction was mined in, if any.</param>
        /// <param name="blockNumber">The block number the transaction was mined in, if any.</param>
        /// <param name="transactionIndex">The index of the transaction in the block.</param>
        /// <param name="gasUsed">The amount of gas the transaction actually used, if it was mined.</param>
        /// <param name="status">The status of the transaction.</param>
        public NetworkTransactionReceiptEntity(TransactionHash transactionHash, BlockHash blockHash, TransactionIndex transactionIndex, BlockNumber blockNumber, GasLimit gasUsed, string status)
        {
            this.TransactionHash = transactionHash ?? throw new ArgumentNullException(nameof(transactionHash));
            this.BlockHash = blockHash ?? throw new ArgumentNullException(nameof(blockHash));
            this.TransactionIndex = transactionIndex ?? throw new ArgumentNullException(nameof(transactionIndex));
            this.BlockNumber = (int) (blockNumber ?? throw new ArgumentNullException(nameof(blockNumber))).Value;
            this.GasUsed = gasUsed ?? throw new ArgumentNullException(nameof(gasUsed));
            this.Status = status;
        }

        /// <summary>
        ///     The transaction hash.
        /// </summary>
        public TransactionHash TransactionHash { get; }

        /// <summary>
        ///     The block hash the transaction was mined in, if any.
        /// </summary>
        public BlockHash BlockHash { get; }

        /// <summary>
        ///     The index of the transaction in the block.
        /// </summary>
        public TransactionIndex TransactionIndex { get; }

        /// <summary>
        ///     The block number the transaction was mined in, if any.
        /// </summary>
        public int BlockNumber { get; }

        /// <summary>
        ///     The amount of gas the transaction actually used, if it was mined.
        /// </summary>
        public GasLimit GasUsed { get; }

        /// <summary>
        ///     The status of the transaction.
        /// </summary>
        public string Status { get; }
    }
}