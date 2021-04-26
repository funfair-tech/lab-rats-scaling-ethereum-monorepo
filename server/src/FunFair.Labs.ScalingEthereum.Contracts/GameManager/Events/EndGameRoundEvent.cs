using FunFair.Ethereum.Contracts.Events;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Events
{
    /// <summary>
    ///     Game ending results.
    /// </summary>
    public sealed class EndGameRoundEvent : Event<EndGameRoundEventOutput>
    {
        /// <inheritdoc />
        public override string Name { get; } = @"EndGameRound";
    }
}