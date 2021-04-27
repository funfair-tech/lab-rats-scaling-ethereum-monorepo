using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FunFair.Common.Environment;
using FunFair.Ethereum.Blocks.Interfaces;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Labs.ScalingEthereum.Authentication;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Logic.Games;
using FunFair.Labs.ScalingEthereum.Logic.Games.EventHandlers;
using FunFair.Labs.ScalingEthereum.Logic.Players;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.RateLimit;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub
{
    /// <summary>
    ///     Authenticated hub
    /// </summary>
    [Authorize(Roles = Role.Dapp)]
    public sealed class AuthenticatedHub : HubBase
    {
        private readonly IEthereumBlockStatus _ethereumBlockStatus;
        private readonly IGameRoundDataManager _gameRoundDataManager;
        private readonly IGameRoundTimeCalculator _gameRoundTimeCalculator;
        private readonly IPlayerCountManager _playerCountManager;
        private readonly IRateLimiter _rateLimiter;
        private readonly IStartRoundGameHistoryBuilder _startRoundGameHistoryBuilder;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="ethereumNetworkRegistry">Ethereum network registry</param>
        /// <param name="groupNameGenerator">The group name generator</param>
        /// <param name="environment">Execution environment</param>
        /// <param name="subscriptionManager">Subscription manager</param>
        /// <param name="playerCountManager">Player counter.</param>
        /// <param name="gameRoundDataManager">Game Round Manager.</param>
        /// <param name="gameRoundTimeCalculator">Game round time calculator.</param>
        /// <param name="startRoundGameHistoryBuilder">Game Round history builder.</param>
        /// <param name="ethereumBlockStatus">Ethereum block status.</param>
        /// <param name="rateLimiter">Rate limiter</param>
        /// <param name="logger">Logger</param>
        public AuthenticatedHub(IEthereumNetworkRegistry ethereumNetworkRegistry,
                                IGroupNameGenerator groupNameGenerator,
                                ExecutionEnvironment environment,
                                ISubscriptionManager subscriptionManager,
                                IPlayerCountManager playerCountManager,
                                IGameRoundDataManager gameRoundDataManager,
                                IGameRoundTimeCalculator gameRoundTimeCalculator,
                                IStartRoundGameHistoryBuilder startRoundGameHistoryBuilder,
                                IEthereumBlockStatus ethereumBlockStatus,
                                IRateLimiter rateLimiter,
                                ILogger<AuthenticatedHub> logger)
            : base(ethereumNetworkRegistry: ethereumNetworkRegistry, groupNameGenerator: groupNameGenerator, environment: environment, subscriptionManager: subscriptionManager, logger: logger)
        {
            this._playerCountManager = playerCountManager ?? throw new ArgumentNullException(nameof(playerCountManager));
            this._gameRoundDataManager = gameRoundDataManager ?? throw new ArgumentNullException(nameof(gameRoundDataManager));
            this._gameRoundTimeCalculator = gameRoundTimeCalculator ?? throw new ArgumentNullException(nameof(gameRoundTimeCalculator));
            this._startRoundGameHistoryBuilder = startRoundGameHistoryBuilder ?? throw new ArgumentNullException(nameof(startRoundGameHistoryBuilder));
            this._ethereumBlockStatus = ethereumBlockStatus ?? throw new ArgumentNullException(nameof(ethereumBlockStatus));
            this._rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
        }

        /// <summary>
        ///     Subscribe
        /// </summary>
        /// <param name="networkName">The network id</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [HubMethodName(name: HubMethodNames.Subscribe)]
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Called by web socket client")]
        public async Task SubscribeAsync(string networkName)
        {
            EthereumNetwork network = this.VerifyNetwork(networkName);

            UserAccountId userAccountId = this.JwtUser()
                                              .Id;

            string connectionId = this.Context.ConnectionId;
            this.Logger.LogInformation($"Client {connectionId} subscribing to network {network.Name}");

            // temporary for dummy data
            await this.SubscribeNetworkAsync(connectionId: this.Context.ConnectionId, network: network);

            this.Logger.LogInformation($"Client {connectionId} subscribed to network {network.Name} and user account id {userAccountId}");

            int playerCount = await this._playerCountManager.AddPlayerAsync(playerId: this.Context.ConnectionId, newNetwork: network);

            await this.Clients.Caller.PlayersOnline(playerCount);

            IReadOnlyList<GameRound> games = await this._gameRoundDataManager.GetAllRunningAsync(network);

            bool anyPublished = false;
            INetworkBlockHeader? networkBlockHeader = this._ethereumBlockStatus.GetLatestBlockRetrievedOnNetwork(network);

            if (networkBlockHeader != null)
            {
                foreach (GameRound game in games)
                {
                    IReadOnlyList<string> gameHistory = await this.GetGameHistoryAsync(game: game);

                    int timeLeft = this._gameRoundTimeCalculator.CalculateTimeLeft(game);

                    await this.Clients.Caller.GameRoundStarted(roundId: game.GameRoundId,
                                                               timeLeftInSeconds: timeLeft,
                                                               blockNumber: game.BlockNumberCreated,
                                                               (int) GameRoundParameters.InterGameDelay.TotalSeconds);

                    await this.Clients.Caller.History(gameHistory);

                    anyPublished = true;
                }
            }

            if (!anyPublished)
            {
                GameRound? gameRound = await this._gameRoundDataManager.GetLastCompletedForNetworkAsync(network);

                if (gameRound != null && networkBlockHeader != null)
                {
                    IReadOnlyList<string> gameHistory = await this.GetGameHistoryAsync(game: gameRound);

                    int timeUntilNextRound = this._gameRoundTimeCalculator.CalculateSecondsUntilNextRound(gameRound);

                    await this.Clients.Caller.LastGameRoundEnded(gameRoundId: gameRound.GameRoundId, startBlockNumber: gameRound.BlockNumberCreated, timeToNextRound: timeUntilNextRound);

                    await this.Clients.Caller.History(gameHistory);
                }
                else
                {
                    await this.Clients.Caller.NoGamesAvailable();
                }
            }
        }

        /// <summary>
        ///     Send message
        /// </summary>
        /// <returns></returns>
        [HubMethodName(name: HubMethodNames.SendMessage)]
        public Task SendMessageAsync(string message)
        {
            AccountAddress accountAddress = this.JwtUser()
                                                .AccountAddress;

            if (!this._rateLimiter.ShouldLimit(this.Context))
            {
                return this.Clients.All.NewMessage(accountAddress: accountAddress, message: message);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Disconnect from hub and remove player from count
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);

            int playerCount = await this._playerCountManager.RemovePlayerAsync(this.Context.ConnectionId);

            await this.Clients.Caller.PlayersOnline(playerCount);
        }

        private async Task<IReadOnlyList<string>> GetGameHistoryAsync(GameRound game)
        {
            IReadOnlyList<GameHistory> history = await this._gameRoundDataManager.GetHistoryAsync(gameContractAddress: game.GameContract.Address,
                                                                                                  maxHistoryItems: NotificationConfiguration.MaxHistoryItems);

            return this._startRoundGameHistoryBuilder.Build(history);
        }
    }
}