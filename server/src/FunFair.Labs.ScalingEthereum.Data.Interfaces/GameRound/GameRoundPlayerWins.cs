using System;
using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound
{
    /// <summary>
    ///     A game round player win
    /// </summary>
    [DebuggerDisplay("GameRoundId: {GameRoundId} AccountAddress: {AccountAddress} WinAmount: {WinAmount} GameRoundId: {DateCreated}")]
    public sealed class GameRoundPlayerWin
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="gameRoundId">The game round id</param>
        /// <param name="accountAddress">Account address</param>
        /// <param name="winAmount">Win amount</param>
        /// <param name="dateCreated">Date created</param>
        public GameRoundPlayerWin(GameRoundId gameRoundId, AccountAddress accountAddress, Token winAmount, DateTime dateCreated)
        {
            this.GameRoundId = gameRoundId ?? throw new ArgumentNullException(nameof(gameRoundId));
            this.AccountAddress = accountAddress ?? throw new ArgumentNullException(nameof(accountAddress));
            this.WinAmount = winAmount ?? throw new ArgumentNullException(nameof(winAmount));
            this.DateCreated = dateCreated;
        }

        /// <summary>
        ///     The game round id
        /// </summary>
        public GameRoundId GameRoundId { get; }

        /// <summary>
        ///     Player wins
        /// </summary>
        public AccountAddress AccountAddress { get; }

        /// <summary>
        ///     Win amount
        /// </summary>
        public Token WinAmount { get; }

        /// <summary>
        ///     Date created
        /// </summary>
        public DateTime DateCreated { get; }
    }
}