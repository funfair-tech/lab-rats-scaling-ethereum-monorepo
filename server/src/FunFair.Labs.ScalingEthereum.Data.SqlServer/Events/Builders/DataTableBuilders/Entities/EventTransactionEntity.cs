using System;
using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders.Entities
{
    /// <summary>
    ///     An entity that contains a transaction hash.
    /// </summary>
    [DebuggerDisplay("{" + nameof(TransactionHash) + "}")]
    public sealed record EventTransactionEntity
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="transactionHash">The transaction hash.</param>
        public EventTransactionEntity(TransactionHash transactionHash)
        {
            this.TransactionHash = transactionHash ?? throw new ArgumentNullException(nameof(transactionHash));
        }

        /// <summary>
        ///     The transaction hash.
        /// </summary>
        public TransactionHash TransactionHash { get; }
    }
}