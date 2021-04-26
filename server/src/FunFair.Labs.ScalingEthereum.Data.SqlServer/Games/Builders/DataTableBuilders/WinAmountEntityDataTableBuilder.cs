using FunFair.Common.Data.Builders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.DataTableBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.DataTableBuilders
{
    /// <summary>
    ///     Data table builder for lists of win amounts.
    /// </summary>
    public sealed class WinAmountEntityDataTableBuilder : SqlDataTableBuilder<WinAmountEntity>
    {
        private static readonly MetadataBuilder Metadata = BuildMetadata();

        /// <summary>
        ///     Constructor.
        /// </summary>
        public WinAmountEntityDataTableBuilder()
            : base(typeName: @"Games.WinAmount", metadataBuilder: Metadata)
        {
        }

        private static MetadataBuilder BuildMetadata()
        {
            MetadataBuilder metadataBuilder = new();

            metadataBuilder.Add(expression: m => m.AccountAddress);
            metadataBuilder.Add(expression: m => m.WinAmount);

            metadataBuilder.Freeze();

            return metadataBuilder;
        }
    }
}