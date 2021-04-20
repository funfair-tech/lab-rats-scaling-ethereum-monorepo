using System.Diagnostics;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.ServiceInterfaces.Models.Transaction;

namespace FunFair.Labs.ScalingEthereum.ServiceInterfaces.Models.Faucet
{
    /// <summary>
    ///     An issuance from the faucet.
    /// </summary>
    [DebuggerDisplay(value: "Eth: {EthAmount} Fun: {TokenAmount} TX: {Transaction.TransactionHash}")]
    public sealed class FaucetDripDto : IFaucetDripDto<PendingTransactionDto>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="ethAmount">The amount of ETH that was issued.</param>
        /// <param name="tokenAmount">The amount of token that was issued.</param>
        /// <param name="transaction">The transaction that was issued to transfer the funds.</param>
        public FaucetDripDto(EthereumAmount ethAmount, Token tokenAmount, PendingTransactionDto transaction)
        {
            this.EthAmount = ethAmount;
            this.TokenAmount = tokenAmount;
            this.Transaction = transaction;
        }

        /// <inheritdoc />
        public EthereumAmount EthAmount { get; }

        /// <inheritdoc />
        public Token TokenAmount { get; }

        /// <inheritdoc />
        public PendingTransactionDto Transaction { get; }
    }
}