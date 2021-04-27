using FunFair.Ethereum.Contracts.Transactions;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Transactions
{
    /// <summary>
    ///     Starts a game round.
    /// </summary>
    public sealed class EndGameRoundBetting : Transaction<StartGameRoundInput>
    {
        /// <inheritdoc />
        public override string Name { get; } = "noMoreBetsForGameRound";
    }
}