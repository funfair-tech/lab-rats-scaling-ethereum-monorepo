using System;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Ethereum.Balances.Interfaces.EventArguments;

namespace FunFair.Labs.ScalingEthereum.Logic.House
{
    /// <summary>
    ///     House Account Alerting
    /// </summary>
    public interface IHouseAccountAlerter
    {
        /// <summary>
        ///     Subscribes to balance change events for the specified contract account
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SubscribeAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Notification of Ethereum balance changes.
        /// </summary>
        event EventHandler<EthereumBalanceChangeEventArgs>? OnEthereumBalanceChanged;
    }
}