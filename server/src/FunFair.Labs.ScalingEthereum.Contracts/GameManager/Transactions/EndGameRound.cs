using FunFair.Ethereum.Contracts.Transactions;

namespace FunFair.Labs.ScalingEthereum.Contracts.GameManager.Transactions
{
    /// <summary>
    ///     Ends a game round.
    /// </summary>
    public sealed class EndGameRound : Transaction<EndGameRoundInput>
    {
        /// <inheritdoc />
        public override string Name { get; } = "endGameRound";
    }
}