using FunFair.Common.Data.Builders;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.ObjectBuilders
{
    /// <summary>
    ///     Builder of <see cref="TransactionHash" /> from, <see cref="EventTransactionHashEntity" /> objects.
    /// </summary>
    public sealed class TransactionHashBuilder : IObjectBuilder<EventTransactionHashEntity, TransactionHash>
    {
        /// <inheritdoc />
        public TransactionHash? Build(EventTransactionHashEntity? source)
        {
            return source?.TransactionHash;
        }
    }
}