using System;
using System.Diagnostics.CodeAnalysis;
using FunFair.Common.Data;
using FunFair.Common.Data.Builders;
using FunFair.Common.Data.Builders.Services;
using FunFair.Common.Data.Interfaces;
using FunFair.Common.Data.Services;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Accounts;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Faucet;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Players;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer
{
    /// <summary>
    ///     Configures the SQL Server services.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class DatabaseSetup
    {
        /// <summary>
        ///     Configures the SQL Server services.
        /// </summary>
        /// <param name="services">The services collection to register services in.</param>
        /// <param name="configuration">The SQL Server configuration.</param>
        public static void Configure(IServiceCollection services, ISqlServerConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            // register configuration
            RegisterConfiguration(services: services, configuration: configuration);

            // Register shared services.
            RegisterDatabaseServices(services);

            SendTransactions.Configure(services);
            EventTransactions.Configure(services);
            ServerAccounts.Configure(services);
            ObjectLocking.Configure(services);
            FaucetTracking.Configure(services);
            Player.Configure(services);
        }

        private static void RegisterDatabaseServices(IServiceCollection services)
        {
            SqlTypeMappingRegistry.RegisterTypes();

            services.AddSingleton<ISqlServerDatabase, SqlServerDatabase>();
            services.AddSingleton<IDatabaseRetrier, SqlDatabaseRetrier>();

            services.AddSingleton(typeof(IObjectCollectionBuilder<,>), typeof(ObjectCollectionBuilder<,>));
        }

        private static void RegisterConfiguration(IServiceCollection services, ISqlServerConfiguration configuration)
        {
            services.AddSingleton(configuration);
        }
    }
}