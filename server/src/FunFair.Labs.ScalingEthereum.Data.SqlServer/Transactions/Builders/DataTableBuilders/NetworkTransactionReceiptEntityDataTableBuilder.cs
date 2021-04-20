using FunFair.Common.Data.Builders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.DataTableBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.DataTableBuilders
{
    /// <summary>
    ///     Builds a table of network transaction receipts.
    /// </summary>
    public sealed class NetworkTransactionReceiptEntityDataTableBuilder : SqlDataTableBuilder<NetworkTransactionReceiptEntity>
    {
        private static readonly MetadataBuilder Metadata = BuildMetadata();

        /// <summary>
        ///     Constructor.
        /// </summary>
        public NetworkTransactionReceiptEntityDataTableBuilder()
            : base(typeName: @"Ethereum.TransactionReceipt", metadataBuilder: Metadata)
        {
        }

        private static MetadataBuilder BuildMetadata()
        {
            MetadataBuilder metadataBuilder = new();

            metadataBuilder.Add(expression: m => m.TransactionHash);
            metadataBuilder.Add(expression: m => m.BlockHash);
            metadataBuilder.Add(expression: m => m.TransactionIndex);
            metadataBuilder.Add(expression: m => m.BlockNumber);
            metadataBuilder.Add(expression: m => m.GasUsed);
            metadataBuilder.Add(expression: m => m.Status);

            metadataBuilder.Freeze();

            return metadataBuilder;
        }
    }
}