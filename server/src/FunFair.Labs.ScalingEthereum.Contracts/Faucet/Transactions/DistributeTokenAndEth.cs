using FunFair.Ethereum.Contracts.Transactions;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Transactions
{
    /// <summary>
    ///     Distributes an amount of token and eth to a recipient from the faucet.
    /// </summary>
    public sealed class DistributeTokenAndEth : Transaction<DistributeTokenAndEthInput>
    {
        /// <inheritdoc />
        public override string Name => @"distributeTokenAndEth";
    }
}