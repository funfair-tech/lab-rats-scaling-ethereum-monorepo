using FunFair.Ethereum.Contracts.Events;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Events
{
    /// <summary>
    ///     Event that signifies that token has been distributed to a recipient.
    /// </summary>
    public sealed class DistributedToken : Event<DistributionOutput>
    {
        /// <inheritdoc />
        public override string Name => @"DistributedToken";
    }
}