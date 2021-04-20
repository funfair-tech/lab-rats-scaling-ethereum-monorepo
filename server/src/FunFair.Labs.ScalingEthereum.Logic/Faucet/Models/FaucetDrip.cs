using System.Diagnostics;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Logic.Faucet.Models
{
    /// <summary>
    ///     An issuance from the faucet.
    /// </summary>
    [DebuggerDisplay(value: "Eth: {EthAmount} Token: {TokenAmount} TX: {Transaction.TransactionHash}")]
    public sealed class FaucetDrip
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="ethAmount">The amount of ETH that was issued.</param>
        /// <param name="tokenAmount">The amount of token that was issued.</param>
        /// <param name="transaction">The transaction that was issued to transfer the funds.</param>
        public FaucetDrip(EthereumAmount ethAmount, Token tokenAmount, PendingTransaction transaction)
        {
            this.EthAmount = ethAmount;
            this.TokenAmount = tokenAmount;
            this.Transaction = transaction;
        }

        /// <summary>
        ///     The amount of ETH that was issued.
        /// </summary>
        public EthereumAmount EthAmount { get; }

        /// <summary>
        ///     The amount of token that was issued.
        /// </summary>
        public Token TokenAmount { get; }

        /// <summary>
        ///     The transaction that was issued to transfer the funds.
        /// </summary>
        public PendingTransaction Transaction { get; }
    }
}