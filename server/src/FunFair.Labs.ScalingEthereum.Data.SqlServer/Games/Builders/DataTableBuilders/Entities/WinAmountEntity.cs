using System;
using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.DataTableBuilders.Entities
{
    /// <summary>
    ///     An entity that contains a win amount,
    /// </summary>
    [DebuggerDisplay("{" + nameof(WinAmount) + "}")]
    public sealed record WinAmountEntity
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="accountAddress">Account address</param>
        /// <param name="winAmount">Win amount.</param>
        public WinAmountEntity(AccountAddress accountAddress, Token winAmount)
        {
            this.AccountAddress = accountAddress ?? throw new ArgumentNullException(nameof(accountAddress));
            this.WinAmount = winAmount ?? throw new ArgumentNullException(nameof(winAmount));
        }

        /// <summary>
        ///     Account address
        /// </summary>
        public AccountAddress AccountAddress { get; }

        /// <summary>
        ///     The win amount.
        /// </summary>
        public Token WinAmount { get; }
    }
}