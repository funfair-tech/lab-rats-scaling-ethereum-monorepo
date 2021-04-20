using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FunFair.Alerts.Dispatching;
using FunFair.Common.Environment;
using FunFair.Common.Environment.Extensions;
using FunFair.Common.Middleware;
using FunFair.Common.ObjectLocking;
using FunFair.Common.Services;
using FunFair.Ethereum.Client.Interfaces;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Ethereum.Proxy.Client;
using FunFair.Labs.ScalingEthereum.Contracts;
using FunFair.Labs.ScalingEthereum.Data.SqlServer;
using FunFair.Labs.ScalingEthereum.DataTypes.Wallet;
using FunFair.Labs.ScalingEthereum.Logic;
using FunFair.Labs.ScalingEthereum.Server.Configuration;
using FunFair.Labs.ScalingEthereum.Server.ServiceStartup;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub;
using FunFair.Labs.ScalingEthereum.ServiceInterfaces;
using FunFair.Random;
using FunFair.Server.Ethereum.Accounts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Server
{
    /// <summary>
    ///     Application setup/start-up
    /// </summary>
    [SuppressMessage(category: "Microsoft.Design", checkId: "CA1052:StaticHolderTypesShouldBeSealed", Justification = "Used for app startup and created by framework.")]
    internal static class ApplicationSetup
    {
        /// <summary>
        ///     Compose the application according to the supplied configuration
        /// </summary>
        /// <param name="services">The service collection to register services into.</param>
        /// <param name="applicationConfiguration">Application configuration.</param>
        public static void Configure(IServiceCollection services, ApplicationConfiguration applicationConfiguration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton(applicationConfiguration);
            services.AddSingleton(typeof(ExecutionEnvironment), implementationInstance: applicationConfiguration.Environment);
            services.AddHttpContextAccessor();

            services.AddSingleton<IRandomSource, DefaultRandomSource>();

            SecurityServices.ConfigureWalletSalt(services: services, walletSaltString: applicationConfiguration.WalletSalt);

            EthereumAccountsSetup.Configure(services);

            string configurationFolder = ApplicationConfigLocator.ConfigurationFilesPath;
            AlertDispatcherSetup.Configure(services: services, configuration: applicationConfiguration.Configuration, pluginsPath: configurationFolder);

            CommonMiddlewareServicesSetup.Configure(services: services);
            CommonServicesSetup.Configure(services);

            ContractsSetup.Configure(services);

            EthereumServices.ConfigureEthereumServices(services: services, configuration: applicationConfiguration);

            DatabaseSetup.Configure(services: services, configuration: applicationConfiguration.SqlServerConfiguration);

            ServiceInterfacesSetup.Configure(services);
            ServiceInterfaceHubSetup.Configure(services);

            services.Configure<WalletConfiguration>(config: applicationConfiguration.Configuration.GetSection(key: @"Wallet"));

            ObjectLockingSetup.Configure(serviceCollection: services, applicationConfiguration.Environment.IsLocalOrTest());

            LogicSetup.Configure(services: services,
                                 faucetConfiguration: applicationConfiguration.FaucetConfiguration,
                                 faucetBalanceConfiguration: applicationConfiguration.FaucetBalanceConfiguration,
                                 houseAlerterConfiguration: applicationConfiguration.HouseBalanceConfiguration);
        }

        public static async Task StartupAsync(IServiceProvider serviceProvider)
        {
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            ExecutionEnvironment environment = serviceProvider.GetRequiredService<ExecutionEnvironment>();
            ApplicationConfiguration configuration = serviceProvider.GetRequiredService<ApplicationConfiguration>();

            Logging.InitializeLogging(environment: environment,
                                      loggerFactory: loggerFactory,
                                      configuration: configuration.LoggingConfiguration,
                                      version: configuration.Version,
                                      tenant: configuration.Tenant);

            // resolve a logger
            ILogger<Startup> logger = loggerFactory.CreateLogger<Startup>();
            logger.LogInformation(new EventId(id: 27), message: "Starting Labs NFT Server");

            EthereumServices.ConfigureGasPriceRecommendationWatcher(serviceProvider: serviceProvider);
            EthereumServices.ConfigureEventProcessing(serviceProvider: serviceProvider);

            try
            {
                await EthereumServices.ClearEventProcessingDatabaseLocksAsync(serviceProvider: serviceProvider, logger: logger);
                await ObjectLockingServices.ClearLocksAsync(serviceProvider: serviceProvider, logger: logger);

                await EnableNetworksAsync(serviceProvider: serviceProvider, logger: logger);

                IConfigurationRoot ethereumConfigurationRoot = EthereumServices.GetEthereumConfiguration(ApplicationConfigLocator.ConfigurationFilesPath);

                IEthereumAccountsMigrator ethereumAccountsMigrator = serviceProvider.GetRequiredService<IEthereumAccountsMigrator>();

                // Must do all of these BEFORE starting all services.
                await ethereumAccountsMigrator.ConfigureEthereumNetworkSettingsAsync(services: serviceProvider,
                                                                                     configuration: ethereumConfigurationRoot,
                                                                                     fateChannelTokenFundingSourceAction: (_, _, _) => { });

                logger.LogInformation(new EventId(id: 28), message: "Labs NFT Services Started");
            }
            catch (Exception exception)
            {
                logger.LogError(new EventId(id: exception.HResult), exception: exception, message: "Failed to start Labs NFT Server services");

                throw;
            }
        }

        /// <summary>
        ///     Called when the application is shutting down
        /// </summary>
        public static Task ShutdownAsync()
        {
            // Ensure any locks that were held are closed by the server on shutdown.
            return Task.WhenAll(ObjectLockingServices.ClearLocksAsync(), EthereumServices.ClearEventProcessingDatabaseLocksAsync());
        }

        private static async Task EnableNetworksAsync(IServiceProvider serviceProvider, ILogger<Startup> logger)
        {
            IWeb3Factory web3Factory = serviceProvider.GetRequiredService<IWeb3Factory>();
            IReadOnlyList<EthereumNetwork> networks = await web3Factory.GetNetworksAsync(CancellationToken.None);
            logger.LogInformation($"There are {networks.Count} networks enabled in proxy.");

            IEthereumNetworkRegistry ethereumNetworkRegistry = serviceProvider.GetRequiredService<IEthereumNetworkRegistry>();
            IReadOnlyList<EthereumNetwork> registeredNetworks = ethereumNetworkRegistry.Supported;
            logger.LogInformation($"There are {registeredNetworks.Count} networks registered in server.");

            IDynamicEthereumNetworkConfigurationManager ethereumNetworkConfigurationManager = serviceProvider.GetRequiredService<IDynamicEthereumNetworkConfigurationManager>();

            foreach (EthereumNetwork network in ethereumNetworkRegistry.Supported)
            {
                ethereumNetworkConfigurationManager.Enable(network);
            }

            IReadOnlyList<EthereumNetwork> configuredNetworks = ethereumNetworkConfigurationManager.EnabledNetworks;
            logger.LogInformation($"There are {configuredNetworks.Count} networks enabled in server.");
        }
    }
}