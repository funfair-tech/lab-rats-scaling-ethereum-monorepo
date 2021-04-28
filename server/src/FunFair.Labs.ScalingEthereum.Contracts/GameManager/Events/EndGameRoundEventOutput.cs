using System;
using System.Diagnostics;
using FunFair.Ethereum.Contracts.Attributes;
using FunFair.Ethereum.Contracts.Events;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Events
{
    /// <summary>
    ///     Event parameters for <see cref="EndGameRoundEvent" />
    /// </summary>
    [DebuggerDisplay("Game Round Id {" + nameof(GameRoundId) + "}")]
    public sealed class EndGameRoundEventOutput : EventOutput, IGameRoundEventOutput
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="gameRoundId">Game round id.</param>
        /// <param name="persistentGameDataId">The persistent game data id.</param>
        /// <param name="entropyReveal">Entropy Reveal.</param>
        /// <param name="players">Player addresses.</param>
        /// <param name="winAmounts">Win amounts.</param>
        /// <param name="progressivePotWinLoss">Progressive Pot Win/Loss.</param>
        /// <param name="gameResult">Game Result (encoded).</param>
        /// <param name="historyToRecord">History to record (encoded).</param>
        public EndGameRoundEventOutput([EventOutputParameter(ethereumDataType: "bytes32", order: 1, indexed: true)]
                                       GameRoundId gameRoundId,
                                       [EventOutputParameter(ethereumDataType: "bytes32", order: 2, indexed: true)]
                                       byte[] persistentGameDataId,
                                       [EventOutputParameter(ethereumDataType: "bytes32", order: 3, indexed: false)]
                                       Seed entropyReveal,
                                       [EventOutputParameter(ethereumDataType: "address[]", order: 4, indexed: false)]
                                       AccountAddress[] players,
                                       [EventOutputParameter(ethereumDataType: "uint256[]", order: 5, indexed: false)]
                                       DataTypes.Primitives.Token[] winAmounts,
                                       [EventOutputParameter(ethereumDataType: "int256", order: 6, indexed: false)]
                                       WinLoss progressivePotWinLoss,
                                       [EventOutputParameter(ethereumDataType: "bytes", order: 7, indexed: false)]
                                       byte[] gameResult,
                                       [EventOutputParameter(ethereumDataType: "bytes", order: 8, indexed: false)]
                                       byte[] historyToRecord)
        {
            this.GameRoundId = gameRoundId ?? throw new ArgumentNullException(nameof(gameRoundId));
            this.PersistentGameDataId = persistentGameDataId ?? throw new ArgumentNullException(nameof(persistentGameDataId));
            this.EntropyReveal = entropyReveal ?? throw new ArgumentNullException(nameof(entropyReveal));
            this.Players = players ?? throw new ArgumentNullException(nameof(players));
            this.WinAmounts = winAmounts ?? throw new ArgumentNullException(nameof(winAmounts));
            this.ProgressivePotWinLoss = progressivePotWinLoss ?? throw new ArgumentNullException(nameof(progressivePotWinLoss));
            this.GameResult = gameResult ?? throw new ArgumentNullException(nameof(gameResult));
            this.History = historyToRecord ?? throw new ArgumentNullException(nameof(historyToRecord));
        }

        /// <summary>
        ///     Entropy Reveal.
        /// </summary>
        public Seed EntropyReveal { get; }

        /// <summary>
        ///     Player addresses
        /// </summary>
        public AccountAddress[] Players { get; }

        /// <summary>
        ///     Win amounts.
        /// </summary>
        public DataTypes.Primitives.Token[] WinAmounts { get; }

        /// <summary>
        ///     Progressive Pot Win/Loss.
        /// </summary>
        public WinLoss ProgressivePotWinLoss { get; }

        /// <summary>
        ///     Game Result (encoded).
        /// </summary>
        public byte[] GameResult { get; }

        /// <summary>
        ///     History to record (encoded).
        /// </summary>
        public byte[] History { get; }

        /// <summary>
        ///     The persistent game data id.
        /// </summary>
        public byte[] PersistentGameDataId { get; }

        /// <inheritdoc />
        public GameRoundId GameRoundId { get; }
    }
}