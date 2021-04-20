using FunFair.Ethereum.Contracts.Events;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Events
{
    /// <summary>
    ///     Event that signifies that ETH has been distributed to a recipient.
    /// </summary>
    public sealed class DistributedEth : Event<DistributionOutput>
    {
        /// <inheritdoc />
        public override string Name => @"DistributedEth";
    }
}