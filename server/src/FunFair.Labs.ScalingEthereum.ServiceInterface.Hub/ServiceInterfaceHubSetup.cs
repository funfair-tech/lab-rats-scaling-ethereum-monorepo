using System;
using System.Diagnostics.CodeAnalysis;
using FunFair.Common.Services;
using FunFair.Labs.ScalingEthereum.Authentication.Events;
using FunFair.Labs.ScalingEthereum.Logic.Players;
using FunFair.Labs.ScalingEthereum.Logic.Players.BackgroundServices;
using FunFair.Labs.ScalingEthereum.Logic.Players.BackgroundServices.Services;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Publishers;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.RateLimit;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.RateLimit.Services;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.Subscription;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub
{
    /// <summary>
    ///     Configures the hub
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceInterfaceHubSetup
    {
        /// <summary>
        ///     Configures the Service interfaces.
        /// </summary>
        /// <param name="services">The services collection to register services in.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Configure(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<JwtEvents>();

            services.AddSingleton<ISubscriptionManager, SubscriptionManager>();
            services.AddSingleton<IGroupNameGenerator, GroupNameGenerator>();
            services.AddSingleton<IPlayerStatisticsPublisher, PLayerStatisticsPublisher>();
            services.AddSingleton<IRateLimiter, RateLimiter>();

            services.AddHostedSingletonService<IOnlinePlayerService, OnlinePlayerService>();
        }
    }
}