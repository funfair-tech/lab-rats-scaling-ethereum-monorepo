using FunFair.Common.Data.Builders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders
{
    /// <inheritdoc />
    /// <summary>
    ///     Data table builder for lists of event transaction hashes.
    /// </summary>
    public sealed class EventTransactionHashDataTableBuilder : SqlDataTableBuilder<EventTransactionEntity>
    {
        private static readonly MetadataBuilder Metadata = BuildMetadata();

        /// <inheritdoc />
        /// <summary>
        ///     Constructor.
        /// </summary>
        public EventTransactionHashDataTableBuilder()
            : base(typeName: @"Ethereum.EventTransactionHash", metadataBuilder: Metadata)
        {
        }

        private static MetadataBuilder BuildMetadata()
        {
            MetadataBuilder metadataBuilder = new();

            metadataBuilder.Add(expression: m => m.TransactionHash);

            metadataBuilder.Freeze();

            return metadataBuilder;
        }
    }
}