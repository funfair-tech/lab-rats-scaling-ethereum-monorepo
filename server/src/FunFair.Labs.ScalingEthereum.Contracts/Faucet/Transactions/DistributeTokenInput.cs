using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FunFair.Ethereum.Contracts.Attributes;
using FunFair.Ethereum.Contracts.Transactions;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Transactions
{
    /// <summary>
    ///     Distributes an amount of token to a recipient.
    /// </summary>
    [DebuggerDisplay(value: "Send {Amount} FUN to {Recipient}")]
    [SuppressMessage(category: "ReSharper", checkId: "ClassNeverInstantiated.Global", Justification = "TODO: Review")]
    public sealed class DistributeTokenInput : TransactionParameters
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="tokenAmount">The token amount to distribute.</param>
        /// <param name="recipient">The recipient of the payments.</param>
        public DistributeTokenInput(DataTypes.Primitives.Token tokenAmount, AccountAddress recipient)
        {
            this.Amount = tokenAmount ?? throw new ArgumentNullException(nameof(tokenAmount));
            this.Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        }

        /// <summary>
        ///     The recipient of the payments.
        /// </summary>
        [InputParameter(order: 1, ethereumDataType: "address")]
        public AccountAddress Recipient { get; }

        /// <summary>
        ///     The token amount to distribute.
        /// </summary>
        [InputParameter(order: 2, ethereumDataType: "uint256")]
        public DataTypes.Primitives.Token Amount { get; }
    }
}