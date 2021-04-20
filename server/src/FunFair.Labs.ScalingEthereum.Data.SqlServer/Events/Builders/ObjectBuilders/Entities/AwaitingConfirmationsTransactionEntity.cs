using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     A 'risky' transaction entity.
    /// </summary>
    [DebuggerDisplay("{Network}: {TransactionHash} Contract: {ContractAddress} Event: {EventSignature}")]
    public sealed record AwaitingConfirmationsTransactionEntity
    {
        /// <summary>The network the transaction is for.</summary>
        public string? Network { get; init; }

        /// <summary>The hash of the transaction.</summary>
        public TransactionHash? TransactionHash { get; init; }

        /// <summary>
        ///     The contract address.
        /// </summary>
        public ContractAddress? ContractAddress { get; init; }

        /// <summary>
        ///     The event signature.
        /// </summary>
        public EventSignature? EventSignature { get; init; }

        /// <summary>
        ///     The event index.
        /// </summary>
        public int EventIndex { get; init; }

        /// <summary>
        ///     The gas used by the transaction.
        /// </summary>
        public GasLimit? GasUsed { get; init; }

        /// <summary>
        ///     The gas price of the transaction.
        /// </summary>
        public GasPrice? GasPrice { get; init; }

        /// <summary>The block number the transaction was first seen.</summary>
        public BlockNumber? BlockNumberFirstSeen { get; init; }

        /// <summary>The number of confirmations to wait for.</summary>
        public int ConfirmationsToWaitFor { get; init; }
    }
}