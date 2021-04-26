using FunFair.Ethereum.Contracts.Events;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Events
{
    /// <summary>
    ///     Game timeout results.
    /// </summary>
    public sealed class StartGameRoundEvent : Event<StartGameRoundEventOutput>
    {
        /// <inheritdoc />
        public override string Name { get; } = @"StartGameRoundIds";
    }
}