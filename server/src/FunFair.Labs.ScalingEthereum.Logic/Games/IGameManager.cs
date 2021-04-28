using System.Threading;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Wallet.Interfaces;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Logic.Games
{
    /// <summary>
    ///     Game manager.
    /// </summary>
    public interface IGameManager
    {
        /// <summary>
        ///     Starts a game.
        /// </summary>
        /// <param name="account">The account to create the game with.</param>
        /// <param name="gameContract">THe game contract to start.</param>
        /// <param name="networkBlockHeader">The block at the time of submitting the transaction.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task StartGameAsync(INetworkSigningAccount account, ContractAddress gameContract, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken);

        /// <summary>
        ///     Starts a game (if the previous start game hasn't successfully executed).
        /// </summary>
        /// <param name="account">The account to create the game with.</param>
        /// <param name="game">The game to start</param>
        /// <param name="networkBlockHeader">The block at the time of submitting the transaction.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task StartGameAsync(INetworkSigningAccount account, GameRound game, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken);

        /// <summary>
        ///     Stops a running game.
        /// </summary>
        /// <param name="account">The account to create end game with (same account that created game).</param>
        /// <param name="gameRoundId">The game round to stop.</param>
        /// <param name="networkBlockHeader">The block at the time of submitting the transaction.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task EndGameAsync(INetworkSigningAccount account, GameRoundId gameRoundId, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken);

        /// <summary>
        ///     Stops a running game.
        /// </summary>
        /// <param name="account">The account to create end game with (same account that created game).</param>
        /// <param name="gameRoundId">The game round to stop betting for.</param>
        /// <param name="networkBlockHeader">The block at the time of submitting the transaction.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task StopBettingAsync(INetworkSigningAccount account, GameRoundId gameRoundId, INetworkBlockHeader networkBlockHeader, CancellationToken cancellationToken);
    }
}