using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Common.ObjectLocking;
using FunFair.Ethereum.Blocks.Interfaces;
using FunFair.Ethereum.Contracts;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Wallet.Interfaces;
using FunFair.Labs.ScalingEthereum.Contracts;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using FunFair.Labs.ScalingEthereum.Logic.Players;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.BackgroundServices.Services
{
    /// <summary>
    ///     Start game services
    /// </summary>
    public sealed class StartGameService : IStartGameService
    {
        private readonly IContractInfo _contractInfo;
        private readonly IEthereumAccountManager _ethereumAccountManager;
        private readonly IEthereumBlockStatus _ethereumBlockStatus;
        private readonly IGameManager _gameManager;

        private readonly IObjectLockManager<EthereumAddress> _gameManagerLockManager;
        private readonly IGameRoundDataManager _gameRoundDataManager;
        private readonly IGamesList _gamesList;
        private readonly ILogger<StartGameService> _logger;
        private readonly IPlayerCountManager _playerCountManager;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="ethereumAccountManager">The ethereum account manager.</param>
        /// <param name="gameManager">Game Manager.</param>
        /// <param name="gameRoundDataManager">Game round data manager.</param>
        /// <param name="ethereumBlockStatus">Ethereum block status.</param>
        /// <param name="gameManagerLockManager">Game contract lock manager.</param>
        /// <param name="contractInfoRegistry">The contract info registry.</param>
        /// <param name="gamesList">Game list.</param>
        /// <param name="playerCountManager">Player count manager.</param>
        /// <param name="logger">Logger</param>
        public StartGameService(IEthereumAccountManager ethereumAccountManager,
                                IGameManager gameManager,
                                IGameRoundDataManager gameRoundDataManager,
                                IEthereumBlockStatus ethereumBlockStatus,
                                IObjectLockManager<EthereumAddress> gameManagerLockManager,
                                IContractInfoRegistry contractInfoRegistry,
                                IGamesList gamesList,
                                IPlayerCountManager playerCountManager,
                                ILogger<StartGameService> logger)
        {
            this._ethereumAccountManager = ethereumAccountManager ?? throw new ArgumentNullException(nameof(ethereumAccountManager));

            this._gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            this._gameRoundDataManager = gameRoundDataManager ?? throw new ArgumentNullException(paramName: nameof(gameRoundDataManager));
            this._ethereumBlockStatus = ethereumBlockStatus ?? throw new ArgumentNullException(nameof(ethereumBlockStatus));
            this._gameManagerLockManager = gameManagerLockManager ?? throw new ArgumentNullException(nameof(gameManagerLockManager));
            this._gamesList = gamesList ?? throw new ArgumentNullException(nameof(gamesList));
            this._playerCountManager = playerCountManager ?? throw new ArgumentNullException(nameof(playerCountManager));
            this._logger = logger ?? throw new ArgumentNullException(paramName: nameof(logger));

            this._contractInfo = contractInfoRegistry.FindContractInfo(WellKnownContracts.GameManager);
        }

        /// <inheritdoc />
        public async Task StartGamesForNetworkAsync(EthereumNetwork network, CancellationToken cancellationToken)
        {
            if (!this._contractInfo.Addresses.ContainsKey(network))
            {
                // Contract not supported on the network
                return;
            }

            INetworkBlockHeader? blockHeader = this._ethereumBlockStatus.GetLatestBlockRetrievedOnNetwork(network);

            if (blockHeader == null)
            {
                // not yet retrieved the latest block so can't do any transactions.
                this._logger.LogWarning($"{network.Name}: No current block - not starting game");

                return;
            }

            int playersOnline = await this._playerCountManager.GetCountAsync(network);

            if (playersOnline == 0)
            {
                this._logger.LogInformation($"{network.Name}: No players online - not starting any games");

                return;
            }

            IReadOnlyList<ContractAddress> gameContracts = this._gamesList.GetGamesForNetwork(network);

            foreach (ContractAddress gameContract in gameContracts)
            {
                await this.StartGameAsync(network: network, gameContract: gameContract, blockHeader: blockHeader, cancellationToken: cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task TryToStartGameAsync(INetworkSigningAccount networkSigningAccount, ContractAddress gameContract, INetworkBlockHeader blockHeader, CancellationToken cancellationToken)
        {
            if (!this._contractInfo.Addresses.TryGetValue(key: networkSigningAccount.Network, out ContractAddress? gameManagerContractAddress))
            {
                // Contract not supported on the network
                return;
            }

            await using (IObjectLock<EthereumAddress>? gameManagerLock = await this._gameManagerLockManager.TakeLockAsync(gameManagerContractAddress))
            {
                if (gameManagerLock == null)
                {
                    // something else has the game manager locked so is probably doing something important with it
                    return;
                }

                bool canGameBeStarted = await this._gameRoundDataManager.CanStartAGameAsync((int) GameRoundParameters.InterGameDelay.TotalSeconds);

                if (!canGameBeStarted)
                {
                    // Has active games don't start a new one
                    return;
                }

                this._logger.LogInformation($"{blockHeader.Network.Name}: Starting new game of game contract {gameContract} using game manager: {gameManagerContractAddress}");

                await this._gameManager.StartGameAsync(account: networkSigningAccount, gameContract: gameContract, networkBlockHeader: blockHeader, cancellationToken: cancellationToken);
            }
        }

        /// <inheritdoc />
        public Task StartGameAsync(EthereumNetwork network, ContractAddress gameContract, INetworkBlockHeader blockHeader, CancellationToken cancellationToken)
        {
            INetworkSigningAccount account = this._ethereumAccountManager.GetAccount(network);

            return this.TryToStartGameAsync(networkSigningAccount: account, gameContract: gameContract, blockHeader: blockHeader, cancellationToken: cancellationToken);
        }
    }
}