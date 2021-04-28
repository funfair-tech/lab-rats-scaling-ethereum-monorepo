using FunFair.Ethereum.Contracts.Transactions;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Transactions
{
    /// <summary>
    ///     Stops betting on a game round.
    /// </summary>
    public sealed class StopBetting : Transaction<StopBettingInput>
    {
        /// <inheritdoc />
        public override string Name { get; } = "noMoreBetsForGameRound";
    }
}