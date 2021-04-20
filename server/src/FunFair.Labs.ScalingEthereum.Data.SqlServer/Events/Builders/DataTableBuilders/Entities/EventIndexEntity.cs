using System;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders.Entities
{
    /// <summary>
    ///     An Event Index.
    /// </summary>
    public sealed record EventIndexEntity
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="contractAddress">The Contract address</param>
        /// <param name="index">The Event index.</param>
        /// <param name="eventSignature">The Event Signature.</param>
        public EventIndexEntity(ContractAddress contractAddress, int index, EventSignature eventSignature)
        {
            this.ContractAddress = contractAddress ?? throw new ArgumentNullException(nameof(contractAddress));
            this.Index = index;
            this.EventSignature = eventSignature ?? throw new ArgumentNullException(nameof(eventSignature));
        }

        /// <summary>
        ///     The contract address.
        /// </summary>
        public ContractAddress ContractAddress { get; }

        /// <summary>
        ///     The Event index.
        /// </summary>
        public int Index { get; }

        /// <summary>
        ///     The Event Signature.
        /// </summary>
        public EventSignature EventSignature { get; }
    }
}