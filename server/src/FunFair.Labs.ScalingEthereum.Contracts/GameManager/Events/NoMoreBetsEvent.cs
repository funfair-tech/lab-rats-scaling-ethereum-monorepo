using FunFair.Ethereum.Contracts.Events;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Events
{
    /// <summary>
    ///     Game round betting should be ended.
    /// </summary>
    public sealed class NoMoreBetsEvent : Event<NoMoreBetsEventOutput>
    {
        /// <inheritdoc />
        public override string Name { get; } = @"NoMoreBets";
    }
}