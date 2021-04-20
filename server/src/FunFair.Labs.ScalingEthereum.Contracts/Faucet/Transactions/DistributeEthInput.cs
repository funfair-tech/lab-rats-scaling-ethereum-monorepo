using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FunFair.Ethereum.Contracts.Attributes;
using FunFair.Ethereum.Contracts.Transactions;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Transactions
{
    /// <summary>
    ///     Distributes an amount of eth to a recipient.
    /// </summary>
    [DebuggerDisplay(value: "Send {Amount} WEI to {Recipient}")]
    [SuppressMessage(category: "ReSharper", checkId: "ClassNeverInstantiated.Global", Justification = "TODO: Review")]
    public sealed class DistributeEthInput : TransactionParameters
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="ethAmount">The Eth amount to distribute.</param>
        /// <param name="recipient">The recipient of the payments.</param>
        public DistributeEthInput(EthereumAmount ethAmount, AccountAddress recipient)
        {
            this.Amount = ethAmount ?? throw new ArgumentNullException(nameof(ethAmount));
            this.Recipient = recipient ?? throw new ArgumentNullException(nameof(recipient));
        }

        /// <summary>
        ///     The recipient of the payments.
        /// </summary>
        [InputParameter(order: 1, ethereumDataType: "address")]
        public AccountAddress Recipient { get; }

        /// <summary>
        ///     The Eth amount to distribute.
        /// </summary>
        [InputParameter(order: 2, ethereumDataType: "uint256")]
        public EthereumAmount Amount { get; }
    }
}