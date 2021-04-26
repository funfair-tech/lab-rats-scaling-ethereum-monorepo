using System;
using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     Game round player win entity
    /// </summary>
    [DebuggerDisplay("GameRoundId: {GameRoundId} WinAmount: {WinAmount} DateCreated: {DateCreated}")]
    public sealed record GameRoundPlayerWinEntity
    {
        /// <summary>
        ///     The game round id
        /// </summary>
        public GameRoundId? GameRoundId { get; init; }

        /// <summary>
        ///     Account address
        /// </summary>
        public AccountAddress? AccountAddress { get; init; }

        /// <summary>
        ///     Win amount
        /// </summary>
        public Token? WinAmount { get; init; }

        /// <summary>
        ///     Date created
        /// </summary>
        public DateTime DateCreated { get; init; }
    }
}