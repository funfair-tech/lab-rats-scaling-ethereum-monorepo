using System;
using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     Event checkpoint
    /// </summary>
    [DebuggerDisplay("{ContractAddress} Start: {StartBlock} Current: {CurrentBlock}")]
    public sealed record EventContractCheckpointEntity
    {
        /// <summary>
        ///     The contract address.
        /// </summary>
        public ContractAddress? ContractAddress { get; init; }

        /// <summary>
        ///     The start  block of synchronization.
        /// </summary>
        public BlockNumber? StartBlock { get; init; }

        /// <summary>
        ///     The current block of synchronization.
        /// </summary>
        public BlockNumber? CurrentBlock { get; init; }

        /// <summary>
        ///     The date/time the status changed.
        /// </summary>
        public DateTime LastUpdated { get; init; }
    }
}