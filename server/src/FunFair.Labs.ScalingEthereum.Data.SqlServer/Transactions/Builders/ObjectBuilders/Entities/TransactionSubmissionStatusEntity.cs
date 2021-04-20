using System;
using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders.Entities
{
    /// <summary>Transaction submission status</summary>
    [DebuggerDisplay(value: "{Network} Account: {Account} Total: {TotalTransactions} Unmined: {UnMinedNonceCount} CurrentNonce: {CurrentNonce}")]
    public sealed record TransactionSubmissionStatusEntity
    {
        /// <summary>The network of the summary.</summary>
        public string? Network { get; init; }

        /// <summary>The address of the summary.</summary>
        public AccountAddress? Account { get; init; }

        /// <summary>
        ///     The total number of transactions that were submitted.
        /// </summary>
        public long TotalTransactions { get; init; }

        /// <summary>The current nonce.</summary>
        public long CurrentNonce { get; init; }

        /// <summary>
        ///     The last nonce that was successfully mined, if any.
        /// </summary>
        public long? LastMinedNonce { get; init; }

        /// <summary>The date/time of the last mined transaction.</summary>
        public DateTime? LastMinedDate { get; init; }

        /// <summary>The first unmined transaction nonce, if any,</summary>
        public long? FirstUnMinedNonce { get; init; }

        /// <summary>
        ///     The date/time of the first un-mined transaction, if any.
        /// </summary>
        public DateTime? FirstUnMinedDate { get; init; }

        /// <summary>The last unmined nonce, if any,</summary>
        public long? LastUnMinedNonce { get; init; }

        /// <summary>
        ///     The date/time of the last un-mined transaction, if any.
        /// </summary>
        public DateTime? LastUnMinedDate { get; init; }

        /// <summary>
        ///     The number of nonces that have not yet been mined.
        /// </summary>
        public long UnMinedNonceCount { get; init; }
    }
}