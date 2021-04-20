using System;
using System.Diagnostics.CodeAnalysis;
using FunFair.Common.Services;
using FunFair.Labs.ScalingEthereum.Logic.Alerts;
using FunFair.Labs.ScalingEthereum.Logic.Alerts.Models;
using FunFair.Labs.ScalingEthereum.Logic.Alerts.Services;
using FunFair.Labs.ScalingEthereum.Logic.Faucet;
using FunFair.Labs.ScalingEthereum.Logic.Faucet.Models;
using FunFair.Labs.ScalingEthereum.Logic.Faucet.Services;
using FunFair.Labs.ScalingEthereum.Logic.Players;
using FunFair.Labs.ScalingEthereum.Logic.Players.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.Logic
{
    /// <summary>
    ///     Configures the Ethereum Contracts
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class LogicSetup
    {
        /// <summary>
        ///     Configures the Ethereum Contracts
        /// </summary>
        /// <param name="services">The services collection to register services in.</param>
        /// <param name="faucetConfiguration">Faucet configuration</param>
        /// <param name="faucetBalanceConfiguration">Faucet balance configuration</param>
        /// <param name="houseAlerterConfiguration">House alerter configuration</param>
        public static void Configure(IServiceCollection services,
                                     IFaucetConfiguration faucetConfiguration,
                                     IFaucetBalanceConfiguration faucetBalanceConfiguration,
                                     IHouseBalanceConfiguration houseAlerterConfiguration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IDrainedFaucetAlerter, DrainedFaucetAlerter>();
            services.AddSingleton<IHouseAccountAlerter, HouseAccountAlerter>();
            services.AddHostedSingletonService<ILowBalanceWatcherService, LowBalanceWatcherService>();

            RegisterPlayers(services);
            RegisterFaucet(services: services, faucetConfiguration: faucetConfiguration, faucetBalanceConfiguration: faucetBalanceConfiguration, houseAlerterConfiguration: houseAlerterConfiguration);
        }

        private static void RegisterPlayers(IServiceCollection services)
        {
            services.AddHostedSingletonService<IPlayerCountManager, PlayerCountManager>();
        }

        private static void RegisterFaucet(IServiceCollection services,
                                           IFaucetConfiguration faucetConfiguration,
                                           IFaucetBalanceConfiguration faucetBalanceConfiguration,
                                           IHouseBalanceConfiguration houseAlerterConfiguration)
        {
            services.AddSingleton<IFaucetManager, FaucetManager>();
            services.AddSingleton(faucetConfiguration);
            services.AddSingleton(faucetBalanceConfiguration);
            services.AddSingleton(houseAlerterConfiguration);
        }
    }
}