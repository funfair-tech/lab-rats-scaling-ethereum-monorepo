using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     A Transaction hash.
    /// </summary>
    [DebuggerDisplay("{" + nameof(TransactionHash) + "}")]
    public sealed record TransactionHashEntity
    {
        /// <summary>
        ///     The transaction hash.
        /// </summary>
        public TransactionHash? TransactionHash { get; init; }
    }
}