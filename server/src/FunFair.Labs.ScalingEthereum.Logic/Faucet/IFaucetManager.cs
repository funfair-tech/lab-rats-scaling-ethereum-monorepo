using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;
using FunFair.Labs.ScalingEthereum.Logic.Faucet.Models;

namespace FunFair.Labs.ScalingEthereum.Logic.Faucet
{
    /// <summary>
    ///     Faucet for Token and native currency.
    /// </summary>
    public interface IFaucetManager
    {
        /// <summary>
        ///     Accesses the faucet.
        /// </summary>
        /// <param name="ipAddress">The IP Address of the client.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="networkBlockHeader">The block at the time of execution.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Details of what was received.</returns>
        Task<FaucetDrip> OpenAsync(IPAddress ipAddress, INetworkAccount recipient, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken);
    }
}