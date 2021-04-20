using FunFair.Common.Data.Builders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders
{
    /// <summary>
    ///     Data table builder for lists of contract addresses.
    /// </summary>
    public sealed class ContractAddressEntityDataTableBuilder : SqlDataTableBuilder<ContractAddressEntity>
    {
        private static readonly MetadataBuilder Metadata = BuildMetadata();

        /// <summary>
        ///     Constructor.
        /// </summary>
        public ContractAddressEntityDataTableBuilder()
            : base(typeName: @"Ethereum.ContractAddress", metadataBuilder: Metadata)
        {
        }

        private static MetadataBuilder BuildMetadata()
        {
            MetadataBuilder metadataBuilder = new();

            metadataBuilder.Add(expression: m => m.ContractAddress);

            metadataBuilder.Freeze();

            return metadataBuilder;
        }
    }
}