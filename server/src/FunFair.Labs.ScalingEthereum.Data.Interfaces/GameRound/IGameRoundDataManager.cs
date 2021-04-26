﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound
{
    /// <summary>
    ///     The game round data manager
    /// </summary>
    public interface IGameRoundDataManager
    {
        /// <summary>
        ///     Gets the game round by id.
        /// </summary>
        /// <param name="gameRoundId">The game round to get.</param>
        /// <returns>The game round, if it exists; otherwise, null.</returns>
        Task<GameRound?> GetAsync(GameRoundId gameRoundId);

        /// <summary>
        ///     Save new game round
        /// </summary>
        /// <param name="gameRoundId">The game round</param>
        /// <param name="createdByAccount">The account that created the pot.</param>
        /// <param name="gameContract">The network contract</param>
        /// <param name="seedCommit">The seed commit</param>
        /// <param name="seedReveal">The seed reveal</param>
        /// <param name="roundDuration">The round duration</param>
        /// <param name="roundTimeoutDuration">The round timeout duration</param>
        /// <param name="blockNumberCreated">Block number</param>
        /// <param name="transactionHash">The transaction that submitted the start round.</param>
        Task SaveStartRoundAsync(GameRoundId gameRoundId,
                                 AccountAddress createdByAccount,
                                 NetworkContract gameContract,
                                 Seed seedCommit,
                                 Seed seedReveal,
                                 TimeSpan roundDuration,
                                 TimeSpan roundTimeoutDuration,
                                 BlockNumber blockNumberCreated,
                                 TransactionHash transactionHash);

        /// <summary>
        ///     Marks the game as being active.
        /// </summary>
        /// <param name="gameRoundId">The game round</param>
        /// <param name="blockNumberCreated">Block number</param>
        /// <param name="activationTime">The time the game was activated/started.</param>
        /// <param name="transactionHash">The transaction that submitted the start round.</param>
        Task ActivateAsync(GameRoundId gameRoundId, BlockNumber blockNumberCreated, DateTime activationTime, TransactionHash transactionHash);

        /// <summary>
        ///     Marks the game as being closed.
        /// </summary>
        /// <param name="gameRoundId">The game round</param>
        /// <param name="blockNumberCreated">Block number</param>
        /// <param name="transactionHash">The transaction that submitted the start round.</param>
        Task BeginCompleteAsync(GameRoundId gameRoundId, BlockNumber blockNumberCreated, TransactionHash transactionHash);

        /// <summary>
        ///     Save end of the round
        /// </summary>
        /// <param name="gameRoundId">The game round</param>
        /// <param name="blockNumberCreated">Block number</param>
        /// <param name="transactionHash">The transaction that submitted the start round.</param>
        /// <param name="winAmounts">Win amounts</param>
        /// <param name="houseWinLoss">House win loss</param>
        /// <param name="progressivePotWinLoss">Progressive pot win loss</param>
        /// <param name="gameResult">Game result</param>
        /// <param name="history">The history data to record.</param>
        Task SaveEndRoundAsync(GameRoundId gameRoundId,
                               BlockNumber blockNumberCreated,
                               TransactionHash transactionHash,
                               WinAmount[] winAmounts,
                               WinLoss houseWinLoss,
                               WinLoss progressivePotWinLoss,
                               byte[] gameResult,
                               byte[] history);

        /// <summary>
        ///     Gets the earliest block number of pots that are being created.
        /// </summary>
        /// <param name="network">The network to query.</param>
        /// <returns>The block number, if there are pending pots; otherwise, null.</returns>
        Task<BlockNumber?> GetEarliestBlockNumberForPendingCreationAsync(EthereumNetwork network);

        /// <summary>
        ///     Gets the earliest block number of pots that are being closed.
        /// </summary>
        /// <param name="network">The network to query.</param>
        /// <returns>The block number, if there are closing pots; otherwise, null.</returns>
        Task<BlockNumber?> GetEarliestBlockNumberForPendingCloseAsync(EthereumNetwork network);

        /// <summary>
        ///     Get all game rounds that can be closed
        /// </summary>
        /// <returns>Collection of games that can be closed on that network.</returns>
        Task<IReadOnlyList<GameRound>> GetAllForClosingAsync(EthereumNetwork network, DateTime dateTimeOnNetwork);

        /// <summary>
        ///     Get the last game round that was completed on the network
        /// </summary>
        /// <param name="network">The network</param>
        /// <returns>The GameRound</returns>
        Task<GameRound?> GetLastCompletedForNetworkAsync(EthereumNetwork network);

        /// <summary>
        ///     Gets all the running games for the progressive pot.
        /// </summary>
        /// <returns>Collection of games.</returns>
        Task<IReadOnlyList<GameRound>> GetAllRunningAsync();

        /// <summary>
        ///     If we can we start a game for a progressive pot
        /// </summary>
        /// <param name="interGameDelay">Inter game delay.</param>
        /// <returns>Collection of games.</returns>
        Task<bool> CanStartAGameAsync(int interGameDelay);

        /// <summary>
        ///     Gets all the running games for the network.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <returns>Collection of games.</returns>
        Task<IReadOnlyList<GameRound>> GetAllRunningAsync(EthereumNetwork network);

        /// <summary>
        ///     Marks the game as being broken.
        /// </summary>
        /// <param name="gameRoundId">The game round.</param>
        /// <param name="closingBlockNumber">Block number.</param>
        /// <param name="exceptionMessage">Reason why it's broken.</param>
        /// <returns></returns>
        Task MarkAsBrokenAsync(GameRoundId gameRoundId, BlockNumber closingBlockNumber, string exceptionMessage);

        /// <summary>
        ///     Game round player wins
        /// </summary>
        /// <param name="gameRoundId">Game round id</param>
        /// <returns>Game round win history data</returns>
        Task<IReadOnlyList<GameRoundPlayerWin>> GetGameRoundPlayerWinsAsync(GameRoundId gameRoundId);

        /// <summary>
        ///     Gets a list of games that need to be fixed.
        /// </summary>
        /// <param name="network">The network.</param>
        /// <param name="dateTimeOnNetwork">The current time on the network</param>
        /// <returns>List of games that need fixing.</returns>
        Task<IReadOnlyList<GameRound>> GetGamesToFixAsync(EthereumNetwork network, DateTime dateTimeOnNetwork);

        /// <summary>
        ///     Gets the history of a game.
        /// </summary>
        /// <param name="gameContractAddress">The game contract address.</param>
        /// <param name="maxHistoryItems">The maximum number of history items to get.</param>
        /// <returns>Collection of history items.</returns>
        Task<IReadOnlyList<GameHistory>> GetHistoryAsync(ContractAddress gameContractAddress, int maxHistoryItems);

        /// <summary>
        ///     Gets a list of transactions
        /// </summary>
        /// <param name="gameRoundId">The game to get transactions for.</param>
        /// <param name="functionName">Function name</param>
        /// <returns>List of transactions.</returns>
        Task<IReadOnlyList<TransactionHash>> GetTransactionsAsync(GameRoundId gameRoundId, string functionName);
    }
}