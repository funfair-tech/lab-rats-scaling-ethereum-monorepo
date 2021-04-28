using System.Threading;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.BackgroundServices
{
    /// <summary>
    ///     Background block service for <see cref="IEndGameBettingBackgroundService" />
    /// </summary>
    public interface IEndGameBettingService
    {
        /// <summary>
        ///     End's the betting for all games according to the time on the network..
        /// </summary>
        /// <param name="blockHeader">The latest block header for the network.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task StopBettingAsync(INetworkBlockHeader blockHeader, CancellationToken cancellationToken);
    }
}