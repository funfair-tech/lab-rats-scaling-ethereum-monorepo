﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Common.Interfaces;
using FunFair.Common.ObjectLocking;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Wallet.Interfaces;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.BackgroundServices.Services
{
    /// <summary>
    ///     Background block service for <see cref="IEndGameBackgroundService" />
    /// </summary>
    public sealed class EndGameService : IEndGameService
    {
        private readonly IDateTimeSource _dateTimeSource;
        private readonly IEthereumAccountManager _ethereumAccountManager;
        private readonly IGameManager _gameManager;
        private readonly IGameRoundDataManager _gameRoundDataManager;
        private readonly IObjectLockManager<GameRoundId> _gameRoundLockManager;
        private readonly ILogger<EndGameService> _logger;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ethereumAccountManager">Ethereum account manager.</param>
        /// <param name="gameRoundDataManager">Game round data manager.</param>
        /// <param name="gameManager">Game Manager.</param>
        /// <param name="gameRoundLockManager">Game round lock manager.</param>
        /// <param name="dateTimeSource">Source of time.</param>
        /// <param name="logger">Logging.</param>
        public EndGameService(IEthereumAccountManager ethereumAccountManager,
                              IGameRoundDataManager gameRoundDataManager,
                              IGameManager gameManager,
                              IObjectLockManager<GameRoundId> gameRoundLockManager,
                              IDateTimeSource dateTimeSource,
                              ILogger<EndGameService> logger)
        {
            this._ethereumAccountManager = ethereumAccountManager ?? throw new ArgumentNullException(nameof(ethereumAccountManager));
            this._gameRoundDataManager = gameRoundDataManager ?? throw new ArgumentNullException(nameof(gameRoundDataManager));
            this._gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            this._gameRoundLockManager = gameRoundLockManager ?? throw new ArgumentNullException(nameof(gameRoundLockManager));
            this._dateTimeSource = dateTimeSource ?? throw new ArgumentNullException(nameof(dateTimeSource));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task EndGameRoundsAsync(INetworkBlockHeader blockHeader, CancellationToken cancellationToken)
        {
            // n.b. can't use the network time as blocks are not regular
            DateTime now = this._dateTimeSource.UtcNow();

            IReadOnlyList<GameRound> gameRoundsToClose = await this._gameRoundDataManager.GetAllForClosingAsync(network: blockHeader.Network, dateTimeOnNetwork: now);

            await Task.WhenAll(gameRoundsToClose.Select(gameRound => this.EndGameRoundAsync(gameRound: gameRound, blockHeader: blockHeader, cancellationToken: cancellationToken)));
        }

        private async Task EndGameRoundAsync(INetworkBlockHeader blockHeader, GameRound gameRound, CancellationToken cancellationToken)
        {
            await using (IObjectLock<GameRoundId>? gameRoundLock = await this._gameRoundLockManager.TakeLockAsync(gameRound.GameRoundId))
            {
                if (gameRoundLock == null)
                {
                    // something else has the game round locked
                    this._logger.LogInformation($"{gameRound.Network.Name}: could not get lock for {gameRound.GameRoundId}");

                    return;
                }

                try
                {
                    INetworkSigningAccount signingAccount = this._ethereumAccountManager.GetAccount(new NetworkAccount(network: gameRound.Network, address: gameRound.CreatedByAccount));

                    this._logger.LogInformation($"{gameRound.Network.Name}: End using game round: {gameRound.GameRoundId}");

                    await this._gameManager.EndGameAsync(account: signingAccount, gameRoundId: gameRound.GameRoundId, networkBlockHeader: blockHeader, cancellationToken: cancellationToken);
                }
                catch (Exception exception)
                {
                    this._logger.LogError(new EventId(exception.HResult), exception: exception, $"{gameRound.Network.Name}: Failed to end game {gameRound.GameRoundId}: {exception.Message}");
                }
            }
        }
    }
}