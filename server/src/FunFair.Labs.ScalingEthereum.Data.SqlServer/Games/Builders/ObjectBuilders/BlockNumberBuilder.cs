using FunFair.Common.Data.Builders;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders.Entities;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders
{
    /// <summary>
    ///     Builder of <see cref="BlockNumberEntity" /> from, <see cref="BlockNumber" /> objects.
    /// </summary>
    public sealed class BlockNumberBuilder : IObjectBuilder<BlockNumberEntity, BlockNumber>
    {
        /// <inheritdoc />
        public BlockNumber? Build(BlockNumberEntity? source)
        {
            return source?.BlockNumber;
        }
    }
}