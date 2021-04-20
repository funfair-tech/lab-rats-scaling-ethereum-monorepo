using FunFair.Ethereum.Contracts.Events;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Events
{
    /// <summary>
    ///     Event that signifies that ETH has been withdrawn from the contract.
    /// </summary>
    public sealed class WithdrewEthFromContract : Event<WithdrawalOutput>
    {
        /// <inheritdoc />
        public override string Name => @"WithdrewEthFromContract";
    }
}