using FunFair.Ethereum.Contracts.Events;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Events
{
    /// <summary>
    ///     Game round betting should be ended.
    /// </summary>
    public sealed class EndGameRoundBettingEvent : Event<EndGameRoundBettingEventOutput>
    {
        /// <inheritdoc />
        public override string Name { get; } = @"EndGameRoundBetting";
    }
}