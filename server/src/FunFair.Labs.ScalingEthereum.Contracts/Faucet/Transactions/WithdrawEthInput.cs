using System;
using System.Diagnostics;
using FunFair.Ethereum.Contracts.Attributes;
using FunFair.Ethereum.Contracts.Transactions;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Transactions
{
    /// <summary>
    ///     Withdraws eth from the faucet.
    /// </summary>
    [DebuggerDisplay(value: "Owner: {" + nameof(OwnerAddress) + "}")]
    public sealed class WithdrawEthInput : TransactionParameters
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ownerAddress">The owner's address.</param>
        public WithdrawEthInput(AccountAddress ownerAddress)
        {
            this.OwnerAddress = ownerAddress ?? throw new ArgumentNullException(nameof(ownerAddress));
        }

        /// <summary>
        ///     The owner's address.
        /// </summary>
        [InputParameter(order: 1, ethereumDataType: @"address")]
        public AccountAddress OwnerAddress { get; }
    }
}