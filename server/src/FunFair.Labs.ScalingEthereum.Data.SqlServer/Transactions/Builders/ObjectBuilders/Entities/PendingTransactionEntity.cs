using System;
using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     Class representing a pending transaction on the Ethereum network
    /// </summary>
    [DebuggerDisplay(value: "{Network} Hash: {TransactionHash} Status: {Status}")]
    public sealed record PendingTransactionEntity
    {
        /// <summary>
        ///     The transaction hash returned from the Ethereum client that submitted the transaction to the network
        /// </summary>
        public TransactionHash? TransactionHash { get; init; }

        /// <summary>
        ///     The network the transaction was submitted to
        /// </summary>
        public string? Network { get; init; }

        /// <summary>
        ///     The Account the transaction was submitted on.
        /// </summary>
        public AccountAddress? Account { get; init; }

        /// <summary>
        ///     The date the transaction was submitted
        /// </summary>
        public DateTime DateSubmitted { get; init; }

        /// <summary>
        ///     The Gas limit for the function.
        /// </summary>
        public GasLimit? GasLimit { get; init; }

        /// <summary>
        ///     Gets the transaction Status
        /// </summary>
        public string? Status { get; init; }

        /// <summary>
        ///     Gets the number of times the transaction has been retried
        /// </summary>
        public int RetryCount { get; init; }

        /// <summary>
        ///     Gets the date/time the transaction was last retried.
        /// </summary>
        public DateTime? DateLastRetried { get; init; }

        /// <summary>The Gas Policy</summary>
        public int GasPolicyExecution { get; init; }

        /// <summary>The nonce for the function.</summary>
        public Nonce? Nonce { get; init; }

        /// <summary>The Gas price for the function.</summary>
        public GasPrice? GasPrice { get; init; }
    }
}