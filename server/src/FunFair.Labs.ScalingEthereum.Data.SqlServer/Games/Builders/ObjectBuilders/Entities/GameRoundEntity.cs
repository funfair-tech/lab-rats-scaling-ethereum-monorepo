using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders.Entities
{
    /// <summary>
    ///     A game round
    /// </summary>
    [DebuggerDisplay("GameRoundId: {GameRoundId} game contract : {GameContract}")]
    public sealed record GameRoundEntity
    {
        /// <summary>
        ///     The game round id
        /// </summary>
        public GameRoundId? GameRoundId { get; init; }

        /// <summary>
        ///     The game contract
        /// </summary>
        public ContractAddress? GameContract { get; init; }

        /// <summary>
        ///     Ethereum network
        /// </summary>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "Initialised by dapper")]
        public string? Network { get; init; }

        /// <summary>
        ///     The commit seed
        /// </summary>
        public Seed? SeedCommit { get; init; }

        /// <summary>
        ///     The reveal seed
        /// </summary>
        public Seed? SeedReveal { get; init; }

        /// <summary>
        ///     Status of game round
        /// </summary>
        public string? Status { get; init; }

        /// <summary>
        ///     The round duration
        /// </summary>
        public int RoundDuration { get; init; }

        /// <summary>
        ///     The round timeout duration
        /// </summary>
        public int RoundTimeoutDuration { get; init; }

        /// <summary>
        ///     The date/time when game round has been created
        /// </summary>
        public DateTime DateCreated { get; init; }

        /// <summary>
        ///     The date/time when game round has been updated
        /// </summary>
        public DateTime DateUpdated { get; init; }

        /// <summary>
        ///     The date/time when game round has been closed
        /// </summary>
        public DateTime? DateClosed { get; init; }

        /// <summary>
        ///     Block number for game round
        /// </summary>
        public BlockNumber? BlockNumberCreated { get; init; }

        /// <summary>
        ///     The account that created the game.
        /// </summary>
        public AccountAddress? CreatedByAccount { get; init; }

        /// <summary>
        ///     The date/time the game was activated.
        /// </summary>
        public DateTime? DateStarted { get; init; }

        /// <summary>
        ///     The Game manager contract
        /// </summary>
        public ContractAddress? GameManagerContract { get; init; }
    }
}