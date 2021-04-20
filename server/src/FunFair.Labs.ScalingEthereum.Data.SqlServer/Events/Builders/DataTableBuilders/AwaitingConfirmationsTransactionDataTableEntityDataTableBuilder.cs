using FunFair.Common.Data.Builders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders
{
    /// <summary>
    ///     Data table builder for lists of risky event transactions..
    /// </summary>
    public sealed class AwaitingConfirmationsTransactionDataTableEntityDataTableBuilder : SqlDataTableBuilder<AwaitingConfirmationsTransactionDataTableEntity>
    {
        private static readonly MetadataBuilder Metadata = BuildMetadata();

        /// <summary>
        ///     Constructor.
        /// </summary>
        public AwaitingConfirmationsTransactionDataTableEntityDataTableBuilder()
            : base(typeName: @"Ethereum.AwaitingConfirmationTransaction", metadataBuilder: Metadata)
        {
        }

        private static MetadataBuilder BuildMetadata()
        {
            MetadataBuilder metadataBuilder = new();

            metadataBuilder.Add(expression: m => m.Network);
            metadataBuilder.Add(expression: m => m.ContractAddress);
            metadataBuilder.Add(expression: m => m.EventSignature);
            metadataBuilder.Add(expression: m => m.TransactionHash);
            metadataBuilder.Add(expression: m => m.EventIndex);
            metadataBuilder.Add(expression: m => m.BlockNumberFirstSeen);
            metadataBuilder.Add(expression: m => m.EarliestBlockNumberForProcessing);
            metadataBuilder.Add(expression: m => m.ConfirmationsToWaitFor);
            metadataBuilder.Add(expression: m => m.GasUsed);
            metadataBuilder.Add(expression: m => m.GasPrice);

            metadataBuilder.Freeze();

            return metadataBuilder;
        }
    }
}