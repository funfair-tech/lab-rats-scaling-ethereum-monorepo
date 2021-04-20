using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     A Nonce in a form that can be stored in MSSQL.
    /// </summary>
    [DebuggerDisplay(value: "Nonce: {" + nameof(Nonce) + "}")]
    public sealed record AccountNonceEntity
    {
        /// <summary>
        ///     The Nonce
        /// </summary>
        public Nonce? Nonce { get; init; }
    }
}