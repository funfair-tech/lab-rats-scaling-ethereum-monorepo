using FunFair.Ethereum.Contracts.Events;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Events
{
    /// <summary>
    ///     Event that signifies that funds have been sent to the faucet.
    /// </summary>
    public sealed class SentFundsToContract : Event<DistributionOutput>
    {
        /// <inheritdoc />
        public override string Name => @"SentFundsToContract";
    }
}