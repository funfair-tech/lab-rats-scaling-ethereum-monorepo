using FunFair.Ethereum.Contracts.Transactions;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Transactions
{
    /// <summary>
    ///     Distributes an amount of eth to a recipient from the faucet.
    /// </summary>
    public sealed class DistributeEth : Transaction<DistributeEthInput>
    {
        /// <inheritdoc />
        public override string Name => @"distributeEth";
    }
}