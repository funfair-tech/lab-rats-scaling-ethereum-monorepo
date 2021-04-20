using FunFair.Common.Data.Builders;
using FunFair.Ethereum.DataTypes;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.DataTableBuilders
{
    /// <summary>
    ///     Data table builder for <see cref="PendingTransaction" />
    /// </summary>
    public sealed class PendingTransactionDataTableBuilder : SqlDataTableBuilder<PendingTransaction>
    {
        private static readonly MetadataBuilder Metadata = BuildMetadata();

        /// <summary>
        ///     Constructor.
        /// </summary>
        public PendingTransactionDataTableBuilder()
            : base(typeName: @"Ethereum.StaleTransaction", metadataBuilder: Metadata)
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