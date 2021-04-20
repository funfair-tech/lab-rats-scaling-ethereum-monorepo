using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Ethereum.DataTypes;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.PlayerCount;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic.Players.Services
{
    /// <inheritdoc cref="IPlayerCountManager" />
    /// <summary>
    ///     Player count manager
    /// </summary>
    public sealed class PlayerCountManager : IPlayerCountManager
    {
        private readonly ILogger<PlayerCountManager> _logger;
        private readonly IPlayerCountDataManager _playerCountDataManager;
        private readonly ConcurrentDictionary<string, EthereumNetwork> _players;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="playerCountDataManager">Player count data manager.</param>
        /// <param name="logger">Logger.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public PlayerCountManager(IPlayerCountDataManager playerCountDataManager, ILogger<PlayerCountManager> logger)
        {
            this._playerCountDataManager = playerCountDataManager ?? throw new ArgumentNullException(nameof(playerCountDataManager));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._players = new ConcurrentDictionary<string, EthereumNetwork>();
        }

        /// <summary>
        ///     Add player into count.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="newNetwork"></param>
        /// <returns></returns>
        public async Task<int> AddPlayerAsync(string playerId, EthereumNetwork newNetwork)
        {
            // is it already registered player we need to check if he changed network or not
            // in case he did we need to decrement count for old network and increment for new.
            if (this._players.TryGetValue(key: playerId, out EthereumNetwork? network))
            {
                if (newNetwork != network)
                {
                    this._logger.LogInformation($"It is existing player, old network: {network} and new network: {newNetwork}");
                    await this._playerCountDataManager.DecrementAsync(machineName: Environment.MachineName, network!);

                    return await this._playerCountDataManager.IncrementAsync(machineName: Environment.MachineName, network: newNetwork);
                }

                this._logger.LogInformation($"It is existing player, no network change, network: {newNetwork}");

                return await this._playerCountDataManager.GetAsync(network);
            }

            this._logger.LogInformation($"New player, increment count for network: {newNetwork}");
            this._players.TryAdd(key: playerId, value: newNetwork);

            return await this._playerCountDataManager.IncrementAsync(machineName: Environment.MachineName, network: newNetwork);
        }

        /// <inheritdoc />
        public Task<int> RemovePlayerAsync(string playerId)
        {
            if (this._players.TryRemove(key: playerId, out EthereumNetwork? network))
            {
                return this._playerCountDataManager.DecrementAsync(machineName: Environment.MachineName, network!);
            }

            throw new InvalidOperationException($"There is no player with {playerId} in players internal list");
        }

        /// <inheritdoc />
        public Task<int> GetCountAsync(EthereumNetwork network)
        {
            return this._playerCountDataManager.GetAsync(network: network);
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return this.ResetPlayerAsync();
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return this.ResetPlayerAsync();
        }

        /// <summary>
        ///     Reset players count for machine to 0
        /// </summary>
        /// <returns></returns>
        private Task ResetPlayerAsync()
        {
            return this._playerCountDataManager.ResetAsync(Environment.MachineName);
        }
    }
}