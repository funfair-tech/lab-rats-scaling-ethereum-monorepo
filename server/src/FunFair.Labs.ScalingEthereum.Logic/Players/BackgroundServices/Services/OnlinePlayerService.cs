using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Common.Services;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Networks.Interfaces;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic.Players.BackgroundServices.Services
{
    /// <summary>
    ///     Background service for online players service
    /// </summary>
    [SuppressMessage(category: "ReSharper", checkId: "ClassNeverInstantiated.Global", Justification = "Instantiated by DI")]
    public sealed class OnlinePlayerService : TickingBackgroundService, IOnlinePlayerService
    {
        private readonly IEthereumNetworkConfigurationManager _ethereumNetworkConfigurationManager;
        private readonly IPlayerStatisticsPublisher _gameStatsPublisher;
        private readonly IPlayerCountManager _playerCountManager;
        private readonly ConcurrentDictionary<EthereumNetwork, int> _playersOnline;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="ethereumNetworkConfigurationManager">Ethereum network configuration manager</param>
        /// <param name="gameStatsPublisher">Game stats publisher</param>
        /// <param name="playerCountManager">The player counter</param>
        /// <param name="logger">The logger</param>
        public OnlinePlayerService(IEthereumNetworkConfigurationManager ethereumNetworkConfigurationManager,
                                   IPlayerStatisticsPublisher gameStatsPublisher,
                                   IPlayerCountManager playerCountManager,
                                   ILogger<OnlinePlayerService> logger)
            : base(TimeSpan.FromSeconds(1), logger: logger)
        {
            this._ethereumNetworkConfigurationManager = ethereumNetworkConfigurationManager;
            this._gameStatsPublisher = gameStatsPublisher ?? throw new ArgumentNullException(nameof(gameStatsPublisher));
            this._playerCountManager = playerCountManager ?? throw new ArgumentNullException(nameof(playerCountManager));
            this._playersOnline = new ConcurrentDictionary<EthereumNetwork, int>();
        }

        /// <inheritdoc />
        protected override async Task TickAsync(CancellationToken cancellationToken)
        {
            foreach (EthereumNetwork network in this._ethereumNetworkConfigurationManager.EnabledNetworks)
            {
                int playerCount = await this._playerCountManager.GetCountAsync(network);

                // assume the count has changed so we send a message on the first tick
                bool updated = true;

                this._playersOnline.AddOrUpdate(key: network,
                                                addValue: playerCount,
                                                updateValueFactory: (_, currentCount) =>
                                                                    {
                                                                        if (playerCount == currentCount)
                                                                        {
                                                                            // count hasn't changed
                                                                            updated = false;
                                                                        }

                                                                        return playerCount;
                                                                    });

                if (updated)
                {
                    await this._gameStatsPublisher.AmountOfPlayersAsync(network: network, players: playerCount);
                }
            }
        }
    }
}