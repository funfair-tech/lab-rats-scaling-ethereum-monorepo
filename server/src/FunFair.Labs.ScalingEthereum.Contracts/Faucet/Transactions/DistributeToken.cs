using FunFair.Ethereum.Contracts.Transactions;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Transactions
{
    /// <summary>
    ///     Distributes an amount of token to a recipient from the faucet.
    /// </summary>
    public sealed class DistributeToken : Transaction<DistributeTokenInput>
    {
        /// <inheritdoc />
        public override string Name => @"distributeToken";
    }
}