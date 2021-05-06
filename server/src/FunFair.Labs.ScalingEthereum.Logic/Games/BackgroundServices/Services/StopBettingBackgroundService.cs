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
    ///     Stop betting service
    /// </summary>
    public sealed class StopBettingBackgroundService : BlockTriggeredBackgroundService, IEndGameBettingBackgroundService
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ethereumNetworkConfigurationManager"></param>
        /// <param name="ethereumBlockStatus"></param>
        /// <param name="latestBlockRetriever">Retriever of the latest blocks.</param>
        /// <param name="dateTimeSource">Source of time</param>
        /// <param name="endGameBettingService">Game betting ending.</param>
        /// <param name="logger">Logging</param>
        public StopBettingBackgroundService(IEthereumNetworkConfigurationManager ethereumNetworkConfigurationManager,
                                            IEthereumBlockStatus ethereumBlockStatus,
                                            IEthereumLatestBlockRetriever latestBlockRetriever,
                                            IDateTimeSource dateTimeSource,
                                            IEndGameBettingService endGameBettingService,
                                            ILogger<StopBettingBackgroundService> logger)
            : base(ethereumNetworkConfigurationManager: ethereumNetworkConfigurationManager,
                   ethereumBlockStatus: ethereumBlockStatus,
                   latestBlockRetriever: latestBlockRetriever,
                   processUnchangedBlocks: false,
                   dateTimeSource: dateTimeSource,
                   sweepInterval: TimeSpan.FromSeconds(0.5),
                   blockTriggeredService: new BackgroundService(endGameBettingService),
                   logger: logger)
        {
        }

        private sealed class BackgroundService : IBlockTriggeredService
        {
            private readonly IEndGameBettingService _endGameBettingService;

            public BackgroundService(IEndGameBettingService endGameBettingService)
            {
                this._endGameBettingService = endGameBettingService ?? throw new ArgumentNullException(nameof(endGameBettingService));
            }

            public Task ProcessNetworkAsync(INetworkBlockHeader blockHeader, bool isLatestBlock, CancellationToken cancellationToken)
            {
                if (!isLatestBlock)
                {
                    return Task.CompletedTask;
                }

                return this._endGameBettingService.StopBettingAsync(blockHeader: blockHeader, cancellationToken: cancellationToken);
            }

            public Task ProcessNetworkForBlockRemovalAsync(INetworkBlockHeader blockHeader, INetworkBlockHeader newBlockHeader, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}