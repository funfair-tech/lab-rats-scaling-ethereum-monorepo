using System.Threading;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Wallet.Interfaces;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.BackgroundServices
{
    /// <summary>
    ///     Start game service for <see cref="IStartGameBackgroundService" />
    /// </summary>
    public interface IStartGameService
    {
        /// <summary>
        ///     Start games for network
        /// </summary>
        /// <param name="network">The ethereum network</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task StartGamesForNetworkAsync(EthereumNetwork network, CancellationToken cancellationToken);

        /// <summary>
        ///     Start game
        /// </summary>
        /// <param name="network">The ethereum network</param>
        /// <param name="gameContract">The game contact</param>
        /// <param name="blockHeader">The block header</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task StartGameAsync(EthereumNetwork network, ContractAddress gameContract, INetworkBlockHeader blockHeader, CancellationToken cancellationToken);

        /// <summary>
        ///     Try to start game
        /// </summary>
        /// <param name="networkSigningAccount">The network signing account</param>
        /// <param name="gameContract">The game contact</param>
        /// <param name="blockHeader">The block header</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        Task TryToStartGameAsync(INetworkSigningAccount networkSigningAccount, ContractAddress gameContract, INetworkBlockHeader blockHeader, CancellationToken cancellationToken);
    }
}