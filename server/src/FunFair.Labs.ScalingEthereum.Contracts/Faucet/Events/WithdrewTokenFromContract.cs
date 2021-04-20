using FunFair.Ethereum.Contracts.Events;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Events
{
    /// <summary>
    ///     Event that signifies that token has been withdrawn from the contract.
    /// </summary>
    public sealed class WithdrewTokenFromContract : Event<WithdrawalOutput>
    {
        /// <inheritdoc />
        public override string Name => @"WithdrewTokenFromContract";
    }
}