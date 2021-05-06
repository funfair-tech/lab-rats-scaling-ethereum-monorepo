using System;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Common.Interfaces;
using FunFair.Ethereum.BackgroundServices;
using FunFair.Ethereum.BackgroundServices.Interfaces;
using FunFair.Ethereum.Blocks.Interfaces;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Networks.Interfaces;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic.Games.BackgroundServices.Services
{
    /// <summary>
    ///     End game service
    /// </summary>
    public sealed class EndGameBackgroundService : BlockTriggeredBackgroundService, IEndGameBackgroundService
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ethereumNetworkConfigurationManager"></param>
        /// <param name="ethereumBlockStatus"></param>
        /// <param name="latestBlockRetriever">Retriever of the latest blocks.</param>
        /// <param name="dateTimeSource">Source of time</param>
        /// <param name="endGameService">Game ending.</param>
        /// <param name="logger">Logging</param>
        public EndGameBackgroundService(IEthereumNetworkConfigurationManager ethereumNetworkConfigurationManager,
                                        IEthereumBlockStatus ethereumBlockStatus,
                                        IEthereumLatestBlockRetriever latestBlockRetriever,
                                        IDateTimeSource dateTimeSource,
                                        IEndGameService endGameService,
                                        ILogger<EndGameBackgroundService> logger)
            : base(ethereumNetworkConfigurationManager: ethereumNetworkConfigurationManager,
                   ethereumBlockStatus: ethereumBlockStatus,
                   latestBlockRetriever: latestBlockRetriever,
                   processUnchangedBlocks: false,
                   dateTimeSource: dateTimeSource,
                   sweepInterval: TimeSpan.FromSeconds(0.5),
                   blockTriggeredService: new BackgroundService(endGameService),
                   logger: logger)
        {
        }

        private sealed class BackgroundService : IBlockTriggeredService
        {
            private readonly IEndGameService _endGameService;

            public BackgroundService(IEndGameService endGameService)
            {
                this._endGameService = endGameService ?? throw new ArgumentNullException(nameof(endGameService));
            }

            public Task ProcessNetworkAsync(INetworkBlockHeader blockHeader, bool isLatestBlock, CancellationToken cancellationToken)
            {
                if (!isLatestBlock)
                {
                    return Task.CompletedTask;
                }

                return this._endGameService.EndGameRoundsAsync(blockHeader: blockHeader, cancellationToken: cancellationToken);
            }

            public Task ProcessNetworkForBlockRemovalAsync(INetworkBlockHeader blockHeader, INetworkBlockHeader newBlockHeader, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}