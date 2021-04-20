using FunFair.Ethereum.Contracts.Transactions;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Transactions
{
    /// <summary>
    ///     Withdraws ETH from the faucet.
    /// </summary>
    public sealed class WithdrawEth : Transaction<WithdrawEthInput>
    {
        /// <inheritdoc />
        public override string Name => @"withdrawEth";
    }
}