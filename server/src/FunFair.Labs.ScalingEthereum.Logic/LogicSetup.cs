using System;
using System.Diagnostics.CodeAnalysis;
using FunFair.Common.Services;
using FunFair.Labs.ScalingEthereum.Logic.Balances;
using FunFair.Labs.ScalingEthereum.Logic.Balances.Services;
using FunFair.Labs.ScalingEthereum.Logic.Faucet;
using FunFair.Labs.ScalingEthereum.Logic.Faucet.Models;
using FunFair.Labs.ScalingEthereum.Logic.Faucet.Services;
using FunFair.Labs.ScalingEthereum.Logic.Games;
using FunFair.Labs.ScalingEthereum.Logic.Games.BackgroundServices;
using FunFair.Labs.ScalingEthereum.Logic.Games.BackgroundServices.Services;
using FunFair.Labs.ScalingEthereum.Logic.Games.EventHandlers;
using FunFair.Labs.ScalingEthereum.Logic.Games.Services;
using FunFair.Labs.ScalingEthereum.Logic.House;
using FunFair.Labs.ScalingEthereum.Logic.House.Services;
using FunFair.Labs.ScalingEthereum.Logic.Players;
using FunFair.Labs.ScalingEthereum.Logic.Players.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.Logic
{
    /// <summary>
    ///     Configures the Server logic
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class LogicSetup
    {
        /// <summary>
        ///     Configures the Server logic
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

            RegisterBalances(services);
            RegisterPlayers(services);
            RegisterFaucet(services: services, faucetConfiguration: faucetConfiguration, faucetBalanceConfiguration: faucetBalanceConfiguration, houseAlerterConfiguration: houseAlerterConfiguration);
            RegisterGames(services);
        }

        private static void RegisterBalances(IServiceCollection services)
        {
            services.AddSingleton<IDrainedFaucetAlerter, DrainedFaucetAlerter>();
            services.AddSingleton<IHouseAccountAlerter, HouseAccountAlerter>();

            services.AddSingleton<ILowBalanceWatcher, LowBalanceWatcher>();
            services.AddHostedSingletonService<ILowBalanceWatcherService, LowBalanceWatcherService>();
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

        private static void RegisterGames(IServiceCollection services)
        {
            services.AddSingleton<IGamesList, GamesList>();
            services.AddSingleton<IGameManager, GameManager>();
            services.AddSingleton<ITransactionService, TransactionService>();

            services.AddSingleton<IStartGameService, StartGameService>();
            services.AddSingleton<IEndGameBettingService, StopBettingService>();
            services.AddSingleton<IEndGameService, EndGameService>();
            services.AddSingleton<IBrokenGameRecovery, BrokenGameRecovery>();
            services.AddSingleton<IStartRoundGameHistoryBuilder, StartRoundGameHistoryBuilder>();
            services.AddSingleton<IGameRoundTimeCalculator, GameRoundTimeCalculator>();

            services.AddHostedSingletonService<IStartGameBackgroundService, StartGameBackgroundService>();
            services.AddHostedSingletonService<IEndGameBettingBackgroundService, StopBettingBackgroundService>();
            services.AddHostedSingletonService<IEndGameBackgroundService, EndGameBackgroundService>();
            services.AddHostedSingletonService<IBrokenGameRecoveryService, BrokenGameRecoveryService>();
        }
    }
}