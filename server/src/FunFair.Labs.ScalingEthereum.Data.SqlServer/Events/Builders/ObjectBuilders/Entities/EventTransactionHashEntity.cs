using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     A Transaction Hash
    /// </summary>
    [DebuggerDisplay("{" + nameof(TransactionHash) + "}")]
    public sealed record EventTransactionHashEntity
    {
        /// <summary>
        ///     The transaction hash
        /// </summary>
        public TransactionHash? TransactionHash { get; init; }
    }
}