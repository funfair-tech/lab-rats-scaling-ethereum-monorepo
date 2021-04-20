using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Accounts.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     Configured account entity
    /// </summary>
    [DebuggerDisplay("Network: {Network} Account: {AccountAddress} Version: {Version}")]
    public sealed record ConfiguredAccountEntity
    {
        /// <summary>
        ///     Whether the account is enabled.
        /// </summary>
        public bool Enabled { get; init; }

        /// <summary>
        ///     The network.
        /// </summary>
        public string? Network { get; init; }

        /// <summary>
        ///     The account address.
        /// </summary>
        public AccountAddress? AccountAddress { get; init; }

        /// <summary>
        ///     The wallet json.
        /// </summary>
        public string? Wallet { get; init; }

        /// <summary>
        ///     The unlock phrase.
        /// </summary>
        public string? Unlock { get; init; }

        /// <summary>
        ///     The Token Funding Account Address.
        /// </summary>
        public AccountAddress? TokenFundingAccountAddress { get; init; }

        /// <summary>
        ///     The encoding version.
        /// </summary>
        public int Version { get; init; }
    }
}