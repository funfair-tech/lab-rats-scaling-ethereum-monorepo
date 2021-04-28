using System.Threading;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.BackgroundServices
{
    /// <summary>
    ///     Background block service for <see cref="IEndGameBackgroundService" />
    /// </summary>
    public interface IEndGameService
    {
        /// <summary>
        ///     End's all the games that are due for ending according to the time on the network.
        /// </summary>
        /// <param name="blockHeader">The latest block header for the network.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task EndGameRoundsAsync(INetworkBlockHeader blockHeader, CancellationToken cancellationToken);
    }
}