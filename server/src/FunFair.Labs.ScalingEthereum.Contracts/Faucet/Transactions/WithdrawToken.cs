using FunFair.Ethereum.Contracts.Transactions;

namespace FunFair.Labs.ScalingEthereum.Contracts.Faucet.Transactions
{
    /// <summary>
    ///     Withdraws token from the faucet.
    /// </summary>
    public sealed class WithdrawToken : Transaction<WithdrawTokenInput>
    {
        /// <inheritdoc />
        public override string Name => @"withdrawToken";
    }
}