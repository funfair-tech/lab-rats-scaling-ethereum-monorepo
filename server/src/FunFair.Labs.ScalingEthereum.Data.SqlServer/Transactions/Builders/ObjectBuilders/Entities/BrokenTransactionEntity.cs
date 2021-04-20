using System;
using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     Base class for broken transactions
    /// </summary>
    [DebuggerDisplay(value: "ID: {TransactionId} Hash:{TransactionHash} Status: {Status} Contract: {ContractAddress} Func: {FunctionName}")]
    public sealed record BrokenTransactionEntity
    {
        /// <summary>The network the transaction was on.</summary>
        public string? Network { get; init; }

        /// <summary>The account that sent the transaction.</summary>
        public AccountAddress? Account { get; init; }

        /// <summary>The transaction ID</summary>
        public long TransactionId { get; init; }

        /// <summary>
        ///     The contract address the transaction was sent to. Note, may be an account address.
        /// </summary>
        public ContractAddress? ContractAddress { get; init; }

        /// <summary>The function name</summary>
        public string? FunctionName { get; init; }

        /// <summary>The hash of the transaction.</summary>
        public TransactionHash? TransactionHash { get; init; }

        /// <summary>Any transaction data.</summary>
        public TransactionData? TransactionData { get; init; }

        /// <summary>Value of the transaction in ETH.</summary>
        public EthereumAmount? Value { get; init; }

        /// <summary>The gas price that was used.</summary>
        public GasPrice? GasPrice { get; init; }

        /// <summary>The Gas limit used by the transaction.</summary>
        public GasLimit? GasLimit { get; init; }

        /// <summary>The nonce of the transaction.</summary>
        public Nonce? Nonce { get; init; }

        /// <summary>The current status of the transaction.</summary>
        public string? Status { get; init; }

        /// <summary>
        ///     The transaction id that this transaction replaced.
        /// </summary>
        public long? ReplacedByTransactionId { get; init; }

        /// <summary>
        ///     The transaction ID that this transaction was replaced by.
        /// </summary>
        public long? ReplacesTransactionId { get; init; }

        /// <summary>The date/time the transaction was submitted.</summary>
        public DateTime DateCreated { get; init; }

        /// <summary>
        ///     The number of times the transaction has been through the resolution process.
        /// </summary>
        public int RetryCount { get; init; }

        /// <summary>Transaction Priority.</summary>
        public string? Priority { get; init; }
    }
}