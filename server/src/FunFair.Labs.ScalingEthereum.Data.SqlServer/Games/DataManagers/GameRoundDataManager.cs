using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunFair.Common.Data;
using FunFair.Common.Data.Builders;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.DataTableBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders.Entities;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.DataManagers
{
    /// <summary>
    ///     Game round data manager
    /// </summary>
    public sealed class GameRoundDataManager : IGameRoundDataManager
    {
        private readonly IObjectBuilder<BlockNumberEntity, BlockNumber> _blockNumberBuilder;
        private readonly ISqlServerDatabase _database;
        private readonly IObjectCollectionBuilder<GameHistoryEntity, GameHistory> _gameHistoryBuilder;
        private readonly IObjectCollectionBuilder<GameRoundEntity, GameRound> _gameRoundBuilder;
        private readonly IObjectCollectionBuilder<GameRoundPlayerWinEntity, GameRoundPlayerWin> _gameRoundPlayerWinsBuilder;
        private readonly IObjectCollectionBuilder<TransactionHashEntity, TransactionHash> _transactionHashBuilder;
        private readonly ISqlDataTableBuilder<WinAmountEntity> _winAmountTableBuilder;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="database">The database to connect to.</param>
        /// <param name="gameRoundBuilder">Game round builder.</param>
        /// <param name="blockNumberBuilder">Block number builder.</param>
        /// <param name="gameRoundWinHistoryBuilder">Game round win history builder</param>
        /// <param name="winAmountTableBuilder">Win amount table builder</param>
        /// <param name="gameHistoryBuilder">Game history builder.</param>
        /// <param name="transactionHashBuilder"></param>
        public GameRoundDataManager(ISqlServerDatabase database,
                                    IObjectCollectionBuilder<GameRoundEntity, GameRound> gameRoundBuilder,
                                    IObjectBuilder<BlockNumberEntity, BlockNumber> blockNumberBuilder,
                                    IObjectCollectionBuilder<GameRoundPlayerWinEntity, GameRoundPlayerWin> gameRoundWinHistoryBuilder,
                                    ISqlDataTableBuilder<WinAmountEntity> winAmountTableBuilder,
                                    IObjectCollectionBuilder<GameHistoryEntity, GameHistory> gameHistoryBuilder,
                                    IObjectCollectionBuilder<TransactionHashEntity, TransactionHash> transactionHashBuilder)
        {
            this._database = database ?? throw new ArgumentNullException(nameof(database));
            this._gameRoundBuilder = gameRoundBuilder ?? throw new ArgumentNullException(nameof(gameRoundBuilder));
            this._blockNumberBuilder = blockNumberBuilder ?? throw new ArgumentNullException(nameof(blockNumberBuilder));
            this._winAmountTableBuilder = winAmountTableBuilder ?? throw new ArgumentNullException(nameof(winAmountTableBuilder));
            this._gameHistoryBuilder = gameHistoryBuilder ?? throw new ArgumentNullException(nameof(gameHistoryBuilder));
            this._transactionHashBuilder = transactionHashBuilder ?? throw new ArgumentNullException(nameof(transactionHashBuilder));
            this._gameRoundPlayerWinsBuilder = gameRoundWinHistoryBuilder ?? throw new ArgumentNullException(nameof(gameRoundWinHistoryBuilder));
        }

        /// <inheritdoc />
        public Task<GameRound?> GetAsync(GameRoundId gameRoundId)
        {
            return this._database.QuerySingleOrDefaultAsync(builder: this._gameRoundBuilder, storedProcedure: @"Games.GameRound_GetById", new {GameRoundId = gameRoundId});
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<GameRound>> GetGamesToFixAsync(EthereumNetwork network, DateTime dateTimeOnNetwork)
        {
            return this._database.QueryAsync(builder: this._gameRoundBuilder, storedProcedure: @"Games.GameRound_GetGamesToFix", new {network = network.Name, dateTimeOnNetwork});
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<GameHistory>> GetHistoryAsync(ContractAddress gameContractAddress, int maxHistoryItems)
        {
            return this._database.QueryAsync(builder: this._gameHistoryBuilder,
                                             storedProcedure: @"Games.GameRound_GetCompletionHistory",
                                             new {GameContract = gameContractAddress, Items = maxHistoryItems});
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<TransactionHash>> GetTransactionsAsync(GameRoundId gameRoundId, string functionName)
        {
            return this._database.QueryAsync(builder: this._transactionHashBuilder, storedProcedure: "Games.GameRound_GetTransactions", new {GameRoundId = gameRoundId, FunctionName = functionName});
        }

        /// <inheritdoc />
        public Task SaveStartRoundAsync(GameRoundId gameRoundId,
                                        AccountAddress createdByAccount,
                                        NetworkContract gameContract,
                                        Seed seedCommit,
                                        Seed seedReveal,
                                        TimeSpan roundDuration,
                                        TimeSpan roundTimeoutDuration,
                                        BlockNumber blockNumberCreated,
                                        TransactionHash transactionHash)
        {
            return this._database.ExecuteAsync(storedProcedure: @"Games.GameRound_Insert",
                                               new
                                               {
                                                   GameRoundId = gameRoundId,
                                                   GameContract = gameContract.Address,
                                                   CreatedByAccount = createdByAccount,
                                                   Network = gameContract.Network.Name,
                                                   BlockNumberCreated = blockNumberCreated,
                                                   SeedCommit = seedCommit,
                                                   SeedReveal = seedReveal,
                                                   RoundDuration = roundDuration.TotalSeconds,
                                                   RoundTimeoutDuration = roundTimeoutDuration.TotalSeconds,
                                                   TransactionHash = transactionHash
                                               });
        }

        /// <inheritdoc />
        public Task ActivateAsync(GameRoundId gameRoundId, BlockNumber blockNumberCreated, DateTime activationTime, TransactionHash transactionHash)
        {
            return this._database.ExecuteAsync(storedProcedure: @"Games.GameRound_Activate",
                                               new {GameRoundId = gameRoundId, BlockNumber = blockNumberCreated, DateStarted = activationTime, TransactionHash = transactionHash});
        }

        /// <inheritdoc />
        public Task BeginCompleteAsync(GameRoundId gameRoundId, BlockNumber blockNumberCreated, TransactionHash transactionHash)
        {
            return this._database.ExecuteAsync(storedProcedure: @"Games.GameRound_BeginComplete", new {GameRoundId = gameRoundId, BlockNumber = blockNumberCreated, TransactionHash = transactionHash});
        }

        /// <inheritdoc />
        public Task SaveEndRoundAsync(GameRoundId gameRoundId,
                                      BlockNumber blockNumberCreated,
                                      TransactionHash transactionHash,
                                      WinAmount[] winAmounts,
                                      WinLoss houseWinLoss,
                                      WinLoss progressivePotWinLoss,
                                      byte[] gameResult,
                                      byte[] history)
        {
            return this._database.ExecuteAsync(storedProcedure: @"Games.GameRound_Complete",
                                               new
                                               {
                                                   GameRoundId = gameRoundId,
                                                   BlockNumber = blockNumberCreated,
                                                   TransactionHash = transactionHash,
                                                   WinAmounts = this._winAmountTableBuilder.Build(winAmounts.Select(Convert)),
                                                   HouseWinLoss = houseWinLoss,
                                                   ProgressivePotWinLoss = progressivePotWinLoss,
                                                   GameResult = gameResult,
                                                   History = history
                                               });
        }

        /// <inheritdoc />
        public Task<BlockNumber?> GetEarliestBlockNumberForPendingCreationAsync(EthereumNetwork network)
        {
            return this._database.QuerySingleOrDefaultAsync(builder: this._blockNumberBuilder, storedProcedure: "Games.GameRound_GetBlockNumberForOpening", new {Network = network.Name});
        }

        /// <inheritdoc />
        public Task<BlockNumber?> GetEarliestBlockNumberForPendingCloseAsync(EthereumNetwork network)
        {
            return this._database.QuerySingleOrDefaultAsync(builder: this._blockNumberBuilder, storedProcedure: "Games.GameRound_GetBlockNumberForClosing", new {Network = network.Name});
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<GameRound>> GetAllForClosingAsync(EthereumNetwork network, DateTime dateTimeOnNetwork)
        {
            return this._database.QueryAsync(builder: this._gameRoundBuilder,
                                             storedProcedure: @"Games.GameRound_GetAllForClosing",
                                             new {Network = network.Name, DateTimeOnNetwork = dateTimeOnNetwork});
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<GameRound>> GetAllRunningAsync()
        {
            return this._database.QueryAsync(builder: this._gameRoundBuilder, storedProcedure: @"Games.GameRound_GetRunning");
        }

        /// <inheritdoc />
        public Task<bool> CanStartAGameAsync(int interGameDelay)
        {
            return this._database.QuerySingleAsync<object, bool>(storedProcedure: @"Games.GameRound_CanStartAGame", new {InterGameDelay = interGameDelay});
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<GameRound>> GetAllRunningAsync(EthereumNetwork network)
        {
            return this._database.QueryAsync(builder: this._gameRoundBuilder, storedProcedure: @"Games.GameRound_GetRunningForNetwork", new {Network = network.Name});
        }

        /// <inheritdoc />
        public Task<GameRound?> GetLastCompletedForNetworkAsync(EthereumNetwork network)
        {
            return this._database.QuerySingleOrDefaultAsync(builder: this._gameRoundBuilder, storedProcedure: @"Games.GameRound_GetLastCompletedForNetwork", new {Network = network.Name});
        }

        /// <inheritdoc />
        public Task MarkAsBrokenAsync(GameRoundId gameRoundId, BlockNumber closingBlockNumber, string exceptionMessage)
        {
            return this._database.ExecuteAsync(storedProcedure: @"Games.GameRound_MarkAsBroken", new {GameRoundId = gameRoundId, BlockNumber = closingBlockNumber, Reason = exceptionMessage});
        }

        private static WinAmountEntity Convert(WinAmount winAmount)
        {
            return new(accountAddress: winAmount.AccountAddress, winAmount: winAmount.Amount);
        }
    }
}