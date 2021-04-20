using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FunFair.Ethereum.Abi;
using FunFair.Ethereum.Abi.Encoder.V1;
using FunFair.Ethereum.Balance.Client;
using FunFair.Ethereum.Balances;
using FunFair.Ethereum.Balances.Service;
using FunFair.Ethereum.Blocks;
using FunFair.Ethereum.Blocks.Service;
using FunFair.Ethereum.BloomFilters;
using FunFair.Ethereum.Calls;
using FunFair.Ethereum.Confirmations;
using FunFair.Ethereum.Confirmations.Interfaces;
using FunFair.Ethereum.Confirmations.Services;
using FunFair.Ethereum.Crypto;
using FunFair.Ethereum.Crypto.Native;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Events;
using FunFair.Ethereum.Events.Data.Interfaces;
using FunFair.Ethereum.Events.Service;
using FunFair.Ethereum.GasPrices;
using FunFair.Ethereum.GasPrices.Interfaces;
using FunFair.Ethereum.GasPrices.Service;
using FunFair.Ethereum.Proxy.Client;
using FunFair.Ethereum.Standard;
using FunFair.Ethereum.Transactions;
using FunFair.Ethereum.Transactions.Interfaces;
using FunFair.Ethereum.Transactions.Service;
using FunFair.Ethereum.Transactions.Services;
using FunFair.Ethereum.Wallet;
using FunFair.Labs.ScalingEthereum.Contracts.Networks;
using FunFair.Labs.ScalingEthereum.Server.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Server.ServiceStartup
{
    internal static class EthereumServices
    {
        private static IEventDataManager? _eventLockManager;

        public static IConfigurationRoot GetEthereumConfiguration(string configurationFolder)
        {
            // load the Ethereum configuration
            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(configurationFolder)
                                                                      .AddJsonFile(path: @"ethereumconfiguration.json", optional: false, reloadOnChange: false)
                                                                      .AddJsonFile(path: @"ethereumconfiguration-local.json", optional: true, reloadOnChange: false);

            return builder.Build();
        }

        public static void ConfigureEthereumServices(IServiceCollection services, ApplicationConfiguration configuration)
        {
            EthereumProxyNetworksSetup.ConfigureDynamic(services: services, PublicEthereumNetworks.KOVAN, Layer2EthereumNetworks.OptimismKovan);

            EthereumProxyClientSetup.Configure(services: services, proxyServerBaseUrl: configuration.Proxy);

            EthereumAbiSetup.Configure(services);
            EthereumAbiEncoderV1Setup.Configure(services);
            EthereumCryptoSetup.Configure<Secp256K1NativeDigitalSignatureAlgorithm>(services);
            EthereumWalletSetup.Configure(services);
            EthereumBloomFiltersSetup.Configure(services);
            EthereumContractCallsSetup.Configure(services);

            EthereumConfirmationsSetup.Configure(services: services, TransactionConfirmationsPolicy());

            // TODO: Add Multi-Call support here

            EnableGasPrices(services);
            EnableBlocks(services);
            EnableBalances(services: services, configuration: configuration);

            EnableEvents(services);
            EnableTransactions(services: services);
        }

        private static void EnableGasPrices(IServiceCollection services)
        {
            EthereumGasPricesSetup.Configure(services);
            EthereumGasPricesServiceSetup.Configure(services);
        }

        private static void EnableBlocks(IServiceCollection services)
        {
            EthereumBlocksSetup.Configure(services);
            EthereumBlocksServiceSetup.Configure(services);
        }

        private static void EnableEvents(IServiceCollection services)
        {
            EthereumEventsSetup.Configure(services);
            EthereumEventsServiceSetup.Configure(services);
        }

        private static void EnableBalances(IServiceCollection services, ApplicationConfiguration configuration)
        {
            EnableBalancesLibrary(services);
            EnableBalancesServer(services: services, configuration: configuration);
        }

        [Conditional("USE_BALANCE_SERVER")]
        private static void EnableBalancesServer(IServiceCollection services, ApplicationConfiguration configuration)
        {
            EthereumBalanceClientSetup.Configure(services: services, balanceServerBaseUrl: configuration.Balances);
        }

        [Conditional("USE_BALANCE_LIB")]
        private static void EnableBalancesLibrary(IServiceCollection services)
        {
            EthereumBalancesSetup.Configure(services);
            EthereumBalancesServiceSetup.Configure(services);
        }

        private static void EnableTransactions(IServiceCollection services)
        {
            services.AddSingleton(TransactionMonitoringConfiguration());
            IGasPriceLimitConfiguration gasPriceLimitConfiguration = new GasPriceLimitConfiguration(maximumExpeditedGasPrice: GasPrice.FromGwei(2m), maximumGasPrice: GasPrice.FromGwei(2m));

            EthereumTransactionsSetup.Configure<RecommendationsBasedGasPricePolicy>(gasLimitPolicy: GasLimitPolicy(),
                                                                                    transactionSpeedUpPolicy: new DefaultTransactionSpeedUpPolicy(gweiToIncreaseBy: 10m),
                                                                                    services: services,
                                                                                    gasPriceLimitConfiguration: gasPriceLimitConfiguration,
                                                                                    recommendationsBasedGasPricePolicyConfiguration: RecommendationsBasedGasPricePolicyConfiguration());
            EthereumTransactionsServiceSetup.Configure(services);
        }

        private static ITransactionConfirmationsPolicy TransactionConfirmationsPolicy()
        {
            return new DefaultTransactionConfirmationsPolicy(highRiskConfirmations: Confirmations.HighRiskTransactionConfirmations,
                                                             standardConfirmations: Confirmations.StandardTransactionConfirmations);
        }

        private static IGasLimitPolicy GasLimitPolicy()
        {
            const int maximumGasLimit = 5000000;

            return new CappedGasLimitPolicy(new GasLimit(maximumGasLimit));
        }

        private static ITransactionMonitoringConfiguration TransactionMonitoringConfiguration()
        {
            return new EthereumTransactionMonitoringConfiguration
                   {
                       StaleThreshold = TimeSpan.FromSeconds(value: 180), NotMiningAlertThreshold = TimeSpan.FromSeconds(value: 420), ResubmitWithHigherGasPrice = TimeSpan.FromSeconds(value: 300)
                   };
        }

        private static IRecommendationsBasedGasPricePolicyConfiguration RecommendationsBasedGasPricePolicyConfiguration()
        {
            return new RecommendationsBasedGasPricePolicyConfiguration(gweiToAdd: 0m, requestedExecutionSpeed: TransactionExecutionSpeed.FAST);
        }

        public static async Task ClearEventProcessingDatabaseLocksAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            logger.LogInformation(message: "Clearing Event Database locks");

            _eventLockManager = serviceProvider.GetRequiredService<IEventDataManager>();

            await ClearEventProcessingDatabaseLocksAsync();

            logger.LogInformation(message: "Clearing Event Database locks - Complete");
        }

        public static Task ClearEventProcessingDatabaseLocksAsync()
        {
            if (_eventLockManager == null)
            {
                return Task.CompletedTask;
            }

            return _eventLockManager.ClearAllLocksAsync();
        }

        [SuppressMessage(category: "Microsoft.Usage", checkId: "CA1801:ReviewUnusedParameters", Justification = "Interface defined for when swagger is enabled.")]
        public static void ConfigureEventProcessing(IServiceProvider serviceProvider)
        {
            // TODO: Implement
#if FALSE
            IContractInfoRegistry registry = serviceProvider.GetRequiredService<IContractInfoRegistry>();
            IContractEventWatcherProcessor processor = serviceProvider.GetRequiredService<IContractEventWatcherProcessor>();

            IContractInfo contractInfo = registry.FindContractInfo(WellKnownContracts.GameManager);

            processor.RegisterEventHandler<CreateProgressivePotEventHandler, CreateProgressivePotEvent, CreateProgressivePotEventOutput>(contractInfo);
            processor.RegisterEventHandler<StartGameRoundEventHandler, StartGameRoundEvent, StartGameRoundEventOutput>(contractInfo);
            processor.RegisterEventHandler<EndGameRoundEventHandler, EndGameRoundEvent, EndGameRoundEventOutput>(contractInfo);
            processor.RegisterEventHandler<TimeoutGameRoundEventHandler, TimeoutGameRoundEvent, TimeoutGameRoundEventOutput>(contractInfo);
#endif
        }

        public static void ConfigureGasPriceRecommendationWatcher(IServiceProvider serviceProvider)
        {
            IGasPriceRecommendationSourceService service = serviceProvider.GetRequiredService<IGasPriceRecommendationSourceService>();

            service.OnGasPriceRecommendationsRetrieved += (_, eventArgs) => { Console.WriteLine($"{eventArgs.Network}: Updated gas price recommendations"); };
        }

        private sealed class EthereumTransactionMonitoringConfiguration : ITransactionMonitoringConfiguration
        {
            /// <inheritdoc />
            public TimeSpan StaleThreshold { get; init; }

            /// <inheritdoc />
            public TimeSpan NotMiningAlertThreshold { get; init; }

            /// <inheritdoc />
            public TimeSpan ResubmitWithHigherGasPrice { get; init; }
        }
    }
}