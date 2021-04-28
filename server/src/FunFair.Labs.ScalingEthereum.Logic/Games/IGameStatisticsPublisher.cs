using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Logic.Games
{
    /// <summary>
    ///     Game stats events publisher
    /// </summary>
    public interface IGameStatisticsPublisher
    {
        /// <summary>
        ///     Game round started data
        /// </summary>
        /// <param name="network">The network the round was started on.</param>
        /// <param name="gameRoundId">The Game round id.</param>
        /// <param name="timeLeftInSeconds">The amount of time left in the round in seconds.</param>
        /// <param name="blockNumber">The block number the round started.</param>
        Task GameRoundStartedAsync(EthereumNetwork network, GameRoundId gameRoundId, int timeLeftInSeconds, BlockNumber blockNumber);

        /// <summary>
        ///     Game round starting
        /// </summary>
        /// <param name="network">The network the game round is starting on</param>
        /// <param name="gameRoundId">The game round id</param>
        /// <param name="transactionHash">The hash of the transaction starting the game</param>
        Task GameRoundStartingAsync(EthereumNetwork network, GameRoundId gameRoundId, TransactionHash transactionHash);

        /// <summary>
        ///     Game round ending
        /// </summary>
        /// <param name="network">The network the game round is ending on</param>
        /// <param name="gameRoundId">The game round id</param>
        /// <param name="transactionHash">The hash of the transaction ending the game</param>
        /// <param name="seedReveal">The seed being revealed in the transaction</param>
        Task GameRoundEndingAsync(EthereumNetwork network, GameRoundId gameRoundId, TransactionHash transactionHash, Seed seedReveal);

        /// <summary>
        ///     Game round ended
        /// </summary>
        /// <param name="network">The network the game round has ended on</param>
        /// <param name="gameRoundId">The game round id</param>
        /// <param name="blockNumber">The block number the game round was ended in</param>
        /// <param name="startBlockNumber">The block number the game round was started in</param>
        Task GameRoundEndedAsync(EthereumNetwork network, GameRoundId gameRoundId, BlockNumber blockNumber, BlockNumber startBlockNumber);

        /// <summary>
        ///     Game round betting is ending
        /// </summary>
        /// <param name="network">The network the game round is ending on</param>
        /// <param name="gameRoundId">The game round id</param>
        /// <param name="transactionHash">The hash of the transaction ending the game</param>
        Task BettingEndingAsync(EthereumNetwork network, GameRoundId gameRoundId, TransactionHash transactionHash);

        /// <summary>
        ///     Game round betting has ended
        /// </summary>
        /// <param name="network">The network the game round has ended on</param>
        /// <param name="gameRoundId">The game round id</param>
        /// <param name="blockNumber">The block number the game round was ended in</param>
        /// <param name="startBlockNumber">The block number the game round was started in</param>
        Task BettingEndedAsync(EthereumNetwork network, GameRoundId gameRoundId, BlockNumber blockNumber, BlockNumber startBlockNumber);

        /// <summary>
        ///     Game round broken
        /// </summary>
        /// <param name="network">The network the game round has broken on</param>
        /// <param name="gameRoundId">The game round id</param>
        Task GameRoundBrokenAsync(EthereumNetwork network, GameRoundId gameRoundId);

        /// <summary>
        ///     Games status
        /// </summary>
        /// <param name="network"></param>
        Task NoGamesAvailableAsync(EthereumNetwork network);
    }
}