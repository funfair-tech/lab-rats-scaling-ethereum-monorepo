using System;
using System.Diagnostics;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound
{
    /// <summary>
    ///     A game round
    /// </summary>
    [DebuggerDisplay("Game Round Id: {GameRoundId} game network contract : {GameContract}")]
    public sealed class GameRound
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="gameRoundId">The game round id</param>
        /// <param name="createdByAccount">The account that created the game.</param>
        /// <param name="network">The network.</param>
        /// <param name="gameManagerContract">The game manager contract.</param>
        /// <param name="gameContract">The game network contract</param>
        /// <param name="seedCommit">The commit seed</param>
        /// <param name="seedReveal">The reveal seed</param>
        /// <param name="status">Status of game round</param>
        /// <param name="roundDuration">Round duration in seconds.</param>
        /// <param name="roundTimeoutDuration">Round timeout duration in seconds.</param>
        /// <param name="dateCreated">The date/time the round was created (transaction submitted)</param>
        /// <param name="dateUpdated">The date/time the round last updated</param>
        /// <param name="dateStarted">The date/time the round was activated (mined)</param>
        /// <param name="dateClosed">The date/time the round was closed.</param>
        /// <param name="blockNumberCreated">The block number the round was activated.</param>
        public GameRound(GameRoundId gameRoundId,
                         AccountAddress createdByAccount,
                         EthereumNetwork network,
                         ContractAddress gameManagerContract,
                         ContractAddress gameContract,
                         Seed seedCommit,
                         Seed seedReveal,
                         GameRoundStatus status,
                         TimeSpan roundDuration,
                         TimeSpan roundTimeoutDuration,
                         DateTime dateCreated,
                         DateTime dateUpdated,
                         DateTime? dateStarted,
                         DateTime? dateClosed,
                         BlockNumber blockNumberCreated)
        {
            this.GameRoundId = gameRoundId ?? throw new ArgumentNullException(nameof(gameRoundId));
            this.CreatedByAccount = createdByAccount ?? throw new ArgumentNullException(nameof(createdByAccount));
            this.Network = network ?? throw new ArgumentNullException(nameof(network));
            this.GameManagerContract = gameManagerContract ?? throw new ArgumentNullException(nameof(gameManagerContract));
            this.GameContract = gameContract ?? throw new ArgumentNullException(nameof(gameContract));
            this.SeedCommit = seedCommit ?? throw new ArgumentNullException(nameof(seedCommit));
            this.SeedReveal = seedReveal ?? throw new ArgumentNullException(nameof(seedReveal));
            this.Status = status;
            this.RoundDuration = roundDuration;
            this.RoundTimeoutDuration = roundTimeoutDuration;
            this.DateCreated = dateCreated;
            this.DateUpdated = dateUpdated;
            this.DateStarted = dateStarted;
            this.DateClosed = dateClosed;
            this.BlockNumberCreated = blockNumberCreated;
        }

        /// <summary>
        ///     The game round id
        /// </summary>
        public GameRoundId GameRoundId { get; }

        /// <summary>
        ///     The game network contract
        /// </summary>
        public ContractAddress GameContract { get; }

        /// <summary>
        ///     The commit seed
        /// </summary>
        public Seed SeedCommit { get; }

        /// <summary>
        ///     The reveal seed
        /// </summary>
        public Seed SeedReveal { get; }

        /// <summary>
        ///     Status of game round
        /// </summary>
        public GameRoundStatus Status { get; }

        /// <summary>
        ///     The round duration
        /// </summary>
        public TimeSpan RoundDuration { get; }

        /// <summary>
        ///     The round timeout duration
        /// </summary>
        public TimeSpan RoundTimeoutDuration { get; }

        /// <summary>
        ///     The date/time when game round has been created
        /// </summary>
        public DateTime DateCreated { get; }

        /// <summary>
        ///     The date/time when game round has been updated
        /// </summary>
        public DateTime DateUpdated { get; }

        /// <summary>
        ///     The date/time when game round has been closed
        /// </summary>
        public DateTime? DateClosed { get; }

        /// <summary>
        ///     Block number for game round
        /// </summary>
        public BlockNumber BlockNumberCreated { get; }

        /// <summary>
        ///     The account that created the game.
        /// </summary>
        public AccountAddress CreatedByAccount { get; }

        public EthereumNetwork Network { get; }

        public ContractAddress GameManagerContract { get; }

        /// <summary>
        ///     The date/time the game was started.
        /// </summary>
        public DateTime? DateStarted { get; }
    }
}