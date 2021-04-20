using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Common.Services;
using FunFair.Labs.ScalingEthereum.Logic.Alerts;
using FunFair.Labs.ScalingEthereum.Logic.Faucet;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Logic
{
    /// <summary>
    ///     Watcher of Casino accounts.
    /// </summary>
    [SuppressMessage(category: "ReSharper", checkId: "ClassNeverInstantiated.Global", Justification = "TODO: Review")]
    public sealed class LowBalanceWatcherService : TickingBackgroundService, ILowBalanceWatcherService
    {
        private readonly IDrainedFaucetAlerter _drainedFaucetAlerter;
        private readonly IHouseAccountAlerter _houseAccountAlerter;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="drainedFaucetAlerter">Drained faucet alerting.</param>
        /// <param name="houseAccountAlerter">House account alerting.</param>
        /// <param name="logger">Logging.</param>
        public LowBalanceWatcherService(IDrainedFaucetAlerter drainedFaucetAlerter, IHouseAccountAlerter houseAccountAlerter, ILogger<LowBalanceWatcherService> logger)
            : base(TimeSpan.FromSeconds(15), logger: logger)
        {
            this._drainedFaucetAlerter = drainedFaucetAlerter ?? throw new ArgumentNullException(nameof(drainedFaucetAlerter));
            this._houseAccountAlerter = houseAccountAlerter ?? throw new ArgumentNullException(nameof(houseAccountAlerter));
        }

        /// <inheritdoc />
        protected override Task TickAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(this._drainedFaucetAlerter.SubscribeAsync(cancellationToken), this._houseAccountAlerter.SubscribeAsync(cancellationToken));
        }
    }
}