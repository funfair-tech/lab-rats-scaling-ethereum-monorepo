using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "Initialised by Dapper")]
        public bool Enabled { get; init; }

        /// <summary>
        ///     The network.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "Initialised by Dapper")]
        public string? Network { get; init; }

        /// <summary>
        ///     The account address.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "Initialised by Dapper")]
        public AccountAddress? AccountAddress { get; init; }

        /// <summary>
        ///     The wallet json.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "Initialised by Dapper")]
        public string? Wallet { get; init; }

        /// <summary>
        ///     The unlock phrase.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "Initialised by Dapper")]
        public string? Unlock { get; init; }

        /// <summary>
        ///     The Token Funding Account Address.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "Initialised by Dapper")]
        public AccountAddress? TokenFundingAccountAddress { get; init; }

        /// <summary>
        ///     The encoding version.
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "Initialised by Dapper")]
        public int Version { get; init; }
    }
}