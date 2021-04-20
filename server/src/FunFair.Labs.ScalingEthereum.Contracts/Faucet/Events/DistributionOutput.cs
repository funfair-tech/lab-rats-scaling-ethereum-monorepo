using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using FunFair.Ethereum.Contracts.Attributes;
using FunFair.Ethereum.Contracts.Events;
using FunFair.Ethereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Events
{
    /// <summary>
    ///     details of a distribution.
    /// </summary>
    [DebuggerDisplay("From: {From} To: {To} Amount: {Amount}")]
    [SuppressMessage(category: "ReSharper", checkId: "ClassNeverInstantiated.Global", Justification = "TODO: Review")]
    public sealed class DistributionOutput : EventOutput
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="from">Sender of the funds.</param>
        /// <param name="to">Recipient of the distribution.</param>
        /// <param name="amount">The amount that was sent.</param>
        public DistributionOutput([EventOutputParameter(ethereumDataType: @"address", order: 1, indexed: false)]
                                  AccountAddress from,
                                  [EventOutputParameter(ethereumDataType: @"address", order: 2, indexed: false)]
                                  AccountAddress to,
                                  [EventOutputParameter(ethereumDataType: @"uint256", order: 3, indexed: false)]
                                  BigInteger amount)
        {
            this.From = from ?? throw new ArgumentNullException(nameof(from));
            this.To = to ?? throw new ArgumentNullException(nameof(to));

            // TODO: Can Amount be FUN/Eth?
            this.Amount = amount;
        }

        /// <summary>
        ///     Sender of the funds.
        /// </summary>
        public AccountAddress From { get; }

        /// <summary>
        ///     Recipient of the distribution.
        /// </summary>
        public AccountAddress To { get; }

        /// <summary>
        ///     The amount that was sent.
        /// </summary>
        public BigInteger Amount { get; }
    }
}