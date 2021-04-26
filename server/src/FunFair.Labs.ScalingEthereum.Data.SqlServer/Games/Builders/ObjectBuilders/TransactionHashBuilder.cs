using FunFair.Common.Data.Builders;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders
{
    /// <summary>
    ///     Builder of <see cref="TransactionHash" /> from <see cref="TransactionHashEntity" /> objects.
    /// </summary>
    public sealed class TransactionHashBuilder : IObjectBuilder<TransactionHashEntity, TransactionHash>
    {
        /// <inheritdoc />
        public TransactionHash? Build(TransactionHashEntity? source)
        {
            return source?.TransactionHash;
        }
    }
}