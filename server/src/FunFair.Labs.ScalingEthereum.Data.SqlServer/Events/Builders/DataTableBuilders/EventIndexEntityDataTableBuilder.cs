using FunFair.Common.Data.Builders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders
{
    /// <summary>
    ///     Data table builder for lists of event indexes.
    /// </summary>
    public sealed class EventIndexEntityDataTableBuilder : SqlDataTableBuilder<EventIndexEntity>
    {
        private static readonly MetadataBuilder Metadata = BuildMetadata();

        /// <summary>
        ///     Constructor.
        /// </summary>
        public EventIndexEntityDataTableBuilder()
            : base(typeName: @"Ethereum.EventIndex", metadataBuilder: Metadata)
        {
        }

        private static MetadataBuilder BuildMetadata()
        {
            MetadataBuilder metadataBuilder = new();

            metadataBuilder.Add(expression: m => m.EventSignature);
            metadataBuilder.Add(expression: m => m.ContractAddress);
            metadataBuilder.Add(expression: m => m.Index);

            metadataBuilder.Freeze();

            return metadataBuilder;
        }
    }
}