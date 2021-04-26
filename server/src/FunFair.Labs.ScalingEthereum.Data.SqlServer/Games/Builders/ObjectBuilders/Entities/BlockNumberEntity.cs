using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     A Block number
    /// </summary>
    [DebuggerDisplay(value: "Block Number: {" + nameof(BlockNumber) + "}")]
    public sealed record BlockNumberEntity
    {
        /// <summary>
        ///     The block number.
        /// </summary>
        public BlockNumber? BlockNumber { get; init; }
    }
}