using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Common.Services;
using FunFair.Ethereum.Networks.Interfaces;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.BackgroundServices.Services
{
    /// <summary>
    ///     Background start game service
    /// </summary>
    public sealed class StartGameBackgroundService : TickingBackgroundService, IStartGameBackgroundService
    {
        private readonly IEthereumNetworkConfigurationManager _ethereumNetworkConfigurationManager;
        private readonly IStartGameService _startGameService;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="startGameService">Start game service.</param>
        /// <param name="ethereumNetworkConfigurationManager">Ethereum network configuration manager.</param>
        /// <param name="logger">The logger.</param>
        public StartGameBackgroundService(IStartGameService startGameService, IEthereumNetworkConfigurationManager ethereumNetworkConfigurationManager, ILogger<StartGameBackgroundService> logger)
            : base(TimeSpan.FromSeconds(1), logger: logger)
        {
            this._startGameService = startGameService ?? throw new ArgumentNullException(nameof(startGameService));
            this._ethereumNetworkConfigurationManager = ethereumNetworkConfigurationManager ?? throw new ArgumentNullException(nameof(ethereumNetworkConfigurationManager));
        }

        /// <inheritdoc />
        protected override Task TickAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(
                this._ethereumNetworkConfigurationManager.EnabledNetworks.Select(network => this._startGameService.StartGamesForNetworkAsync(network: network, cancellationToken: cancellationToken)));
        }
    }
}