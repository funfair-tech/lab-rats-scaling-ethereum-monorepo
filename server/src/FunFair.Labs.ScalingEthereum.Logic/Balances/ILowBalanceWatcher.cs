using FunFair.Ethereum.DataTypes;

namespace FunFair.Labs.ScalingEthereum.Logic.Balances
{
    /// <summary>
    ///     Watching accounts if they have enough balance to start transactions
    /// </summary>
    public interface ILowBalanceWatcher
    {
        bool DoesAccountHaveEnoughBalance(INetworkAccount networkAccount);
    }
}