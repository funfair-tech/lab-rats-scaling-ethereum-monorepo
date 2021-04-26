using System.Threading;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.BackgroundServices
{
    /// <summary>
    ///     Recovery of broken games.
    /// </summary>
    public interface IBrokenGameRecovery
    {
        /// <summary>
        ///     Recovers any broken games.
        /// </summary>
        /// <param name="blockHeader">The current block header for the network.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task RecoverAsync(INetworkBlockHeader blockHeader, CancellationToken cancellationToken);
    }
}