using System;
using System.Threading.Tasks;
using FunFair.Common.ObjectLocking.Data.Interfaces;
using FunFair.Ethereum.DataTypes.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Server.ServiceStartup
{
    internal static class ObjectLockingServices
    {
        private static IObjectLockDataManager<EthereumAddress>? _gameManagerLockDataManager;
        private static ILogger<Startup>? _logger;

        public static Task ClearLocksAsync()
        {
            return Task.WhenAll(ClearGameManagerLockAsync());
        }

        public static Task ClearLocksAsync(IServiceProvider serviceProvider, ILogger<Startup> logger)
        {
            _logger = logger;
            _gameManagerLockDataManager = serviceProvider.GetRequiredService<IObjectLockDataManager<EthereumAddress>>();

            return Task.WhenAll(ClearGameManagerLockAsync());
        }

        private static Task ClearGameManagerLockAsync()
        {
            _logger?.LogInformation("Clearing locks for game manager");

            return _gameManagerLockDataManager?.ClearAllLocksAsync() ?? Task.CompletedTask;
        }
    }
}