using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using FunFair.Ethereum.Contracts.Attributes;
using FunFair.Ethereum.Contracts.Events;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Events
{
    /// <summary>
    ///     Event output for the <see cref="WithdrewEthFromContract" /> or <see cref="WithdrewTokenFromContract" /> events.
    /// </summary>
    [DebuggerDisplay("To: {To} Amount: {Amount}")]
    [SuppressMessage(category: "ReSharper", checkId: "ClassNeverInstantiated.Global", Justification = "Instantiated by ethereum library")]
    public sealed class WithdrawalOutput : EventOutput
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="to">The address the funds were sent to.</param>
        /// <param name="amount">The amount that was sent.</param>
        public WithdrawalOutput([EventOutputParameter(ethereumDataType: "address", order: 1, indexed: false)]
                                AccountAddress to,
                                [EventOutputParameter(ethereumDataType: "uint256", order: 2, indexed: false)]
                                DataTypes.Primitives.Token amount)
        {
            this.To = to ?? throw new ArgumentNullException(nameof(to));
            this.Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        }

        /// <summary>
        ///     The address the funds were sent to.
        /// </summary>
        public AccountAddress To { get; }

        /// <summary>
        ///     The amount that was sent.
        /// </summary>
        public DataTypes.Primitives.Token Amount { get; }
    }
}