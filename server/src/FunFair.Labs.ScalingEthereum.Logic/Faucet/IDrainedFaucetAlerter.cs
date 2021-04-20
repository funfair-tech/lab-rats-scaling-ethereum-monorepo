using System.Threading;
using System.Threading.Tasks;

namespace FunFair.Labs.ScalingEthereum.Logic.Faucet
{
    /// <summary>
    ///     Issues alerts when monitored faucet contracts are low on Eth or TOKEN.
    /// </summary>
    public interface IDrainedFaucetAlerter
    {
        /// <summary>
        ///     Subscribes to balance change events.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task SubscribeAsync(CancellationToken cancellationToken);
    }
}