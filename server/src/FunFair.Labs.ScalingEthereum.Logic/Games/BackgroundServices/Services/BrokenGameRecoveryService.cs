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
    ///     Background service for recovering broken games.
    /// </summary>
    public sealed class BrokenGameRecoveryService : BlockTriggeredBackgroundService, IBrokenGameRecoveryService
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="ethereumNetworkConfigurationManager"></param>
        /// <param name="ethereumBlockStatus"></param>
        /// <param name="latestBlockRetriever">Retriever of the latest blocks.</param>
        /// <param name="dateTimeSource">Source of time</param>
        /// <param name="brokenGameRecovery">Broken game recovery.</param>
        /// <param name="logger">Logging</param>
        public BrokenGameRecoveryService(IEthereumNetworkConfigurationManager ethereumNetworkConfigurationManager,
                                         IEthereumBlockStatus ethereumBlockStatus,
                                         IEthereumLatestBlockRetriever latestBlockRetriever,
                                         IDateTimeSource dateTimeSource,
                                         IBrokenGameRecovery brokenGameRecovery,
                                         ILogger<BrokenGameRecoveryService> logger)
            : base(ethereumNetworkConfigurationManager: ethereumNetworkConfigurationManager,
                   ethereumBlockStatus: ethereumBlockStatus,
                   latestBlockRetriever: latestBlockRetriever,
                   processUnchangedBlocks: false,
                   dateTimeSource: dateTimeSource,
                   sweepInterval: TimeSpan.FromSeconds(30),
                   blockTriggeredService: new BackgroundService(brokenGameRecovery),
                   logger: logger)
        {
        }

        private sealed class BackgroundService : IBlockTriggeredService
        {
            private readonly IBrokenGameRecovery _brokenGameRecovery;

            public BackgroundService(IBrokenGameRecovery brokenGameRecovery)
            {
                this._brokenGameRecovery = brokenGameRecovery ?? throw new ArgumentNullException(nameof(brokenGameRecovery));
            }

            /// <inheritdoc />
            public Task ProcessNetworkAsync(INetworkBlockHeader blockHeader, bool isLatestBlock, CancellationToken cancellationToken)
            {
                return this._brokenGameRecovery.RecoverAsync(blockHeader: blockHeader, cancellationToken: cancellationToken);
            }

            /// <inheritdoc />
            public Task ProcessNetworkForBlockRemovalAsync(INetworkBlockHeader blockHeader, INetworkBlockHeader newBlockHeader, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}