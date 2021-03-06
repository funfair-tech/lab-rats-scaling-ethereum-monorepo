using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using Microsoft.AspNetCore.SignalR;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub
{
    /// <summary>
    ///     A hub
    /// </summary>
    [SuppressMessage(category: "Threading", checkId: "VSTHRD200:Use Async Suffix in names of method that return an awaitable type", Justification = "Hub Method names should not include async")]
    public interface IHub
    {
        /// <summary>
        ///     A message was logged.
        /// </summary>
        /// <param name="connectionId">The connection Id.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        [HubMethodName(HubEventNames.Log)]
        Task Log(string connectionId, string message);

        /// <summary>
        ///     Send the players online data
        /// </summary>
        /// <param name="players">Amount of players</param>
        [HubMethodName(HubEventNames.PlayersOnline)]
        Task PlayersOnline(int players);

        /// <summary>
        ///     Broadcast new chat message.
        /// </summary>
        /// <param name="accountAddress">User's account address.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        [HubMethodName(HubEventNames.NewMessage)]
        Task NewMessage(AccountAddress accountAddress, string message);

        /// <summary>
        ///     Game round started
        /// </summary>
        /// <param name="roundId">Game Round id</param>
        /// <param name="timeLeftInSeconds">Time left in seconds.</param>
        /// <param name="blockNumber">The block number the game was started in.</param>
        /// <param name="interGameDelay">The time the server will wait before starting a new round after this one has finished, in seconds.</param>
        [HubMethodName(HubEventNames.GameRoundStarted)]
        Task GameRoundStarted(GameRoundId roundId, int timeLeftInSeconds, BlockNumber blockNumber, int interGameDelay);

        /// <summary>
        ///     Game round history
        /// </summary>
        /// <param name="historyEntries">Game history entries.</param>
        [HubMethodName(HubEventNames.History)]
        Task History(IReadOnlyList<string> historyEntries);

        /// <summary>
        ///     Game round starting
        /// </summary>
        /// <param name="roundId">Game Round id</param>
        /// <param name="transactionHash">Transaction hash for the transaction which will start the round</param>
        [HubMethodName(HubEventNames.GameRoundStarting)]
        Task GameRoundStarting(GameRoundId roundId, TransactionHash transactionHash);

        /// <summary>
        ///     Game round betting ending.
        /// </summary>
        /// <param name="roundId">Game Round id</param>
        /// <param name="transactionHash">Transaction hash for the transaction which will end the round</param>
        [HubMethodName(HubEventNames.BettingEnding)]
        Task BettingEnding(GameRoundId roundId, TransactionHash transactionHash);

        /// <summary>
        ///     Game round ended
        /// </summary>
        /// <param name="roundId">Game Round id</param>
        /// <param name="blockNumber">The block number the game was ended in.</param>
        [HubMethodName(HubEventNames.BettingEnded)]
        Task BettingEnded(GameRoundId roundId, BlockNumber blockNumber);

        /// <summary>
        ///     Game round ending
        /// </summary>
        /// <param name="roundId">Game Round id</param>
        /// <param name="transactionHash">Transaction hash for the transaction which will end the round</param>
        /// <param name="seedReveal">The Seed being revealed.</param>
        [HubMethodName(HubEventNames.GameRoundEnding)]
        Task GameRoundEnding(GameRoundId roundId, TransactionHash transactionHash, Seed seedReveal);

        /// <summary>
        ///     Game round ended
        /// </summary>
        /// <param name="roundId">Game Round id</param>
        /// <param name="blockNumber">The block number the game was ended in.</param>
        /// <param name="interGameDelay">The time the server will wait before starting a new round, in seconds.</param>
        /// <param name="startBlockNumber">The block the game was started in</param>
        [HubMethodName(HubEventNames.GameRoundEnded)]
        Task GameRoundEnded(GameRoundId roundId, BlockNumber blockNumber, int interGameDelay, BlockNumber startBlockNumber);

        /// <summary>
        ///     Game round broken
        /// </summary>
        /// <param name="roundId">Game Round id</param>
        /// <param name="interGameDelay">The time the server will wait before starting a new round, in seconds.</param>
        [HubMethodName(HubEventNames.GameRoundBroken)]
        Task GameRoundBroken(GameRoundId roundId, int interGameDelay);

        /// <summary>
        ///     Last game round ended
        /// </summary>
        /// <param name="gameRoundId">Game round id</param>
        /// <param name="startBlockNumber">The block number the game started in</param>
        /// <param name="timeToNextRound">The number of seconds until the next round will start</param>
        [HubMethodName(HubEventNames.LastGameRoundEnded)]
        Task LastGameRoundEnded(GameRoundId gameRoundId, BlockNumber startBlockNumber, int timeToNextRound);

        /// <summary>
        ///     No games available
        /// </summary>
        [HubMethodName(HubEventNames.NoGamesAvailable)]
        Task NoGamesAvailable();

        /// <summary>
        ///     Some funds were issued from the faucet.
        /// </summary>
        /// <param name="nativeCurrencyAmount">The amount of the native currency that was issued.</param>
        /// <param name="tokenAmount">The amount of the token that was issued.</param>
        /// <param name="transactionHash">The transaction hash of the transaction that issued the funds.</param>
        [HubMethodName(HubEventNames.FaucetDrip)]
        Task FaucetDrip(EthereumAmount nativeCurrencyAmount, Token tokenAmount, TransactionHash transactionHash);
    }
}