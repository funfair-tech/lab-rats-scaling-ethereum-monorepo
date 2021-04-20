using System.Diagnostics.CodeAnalysis;
using FunFair.Common.Data.Builders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Accounts.Builders.ObjectBuilders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Accounts.Builders.ObjectBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Accounts.DataManagers;
using FunFair.Server.Ethereum.Accounts.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Accounts
{
    [ExcludeFromCodeCoverage]
    internal static class ServerAccounts
    {
        public static void Configure(IServiceCollection services)
        {
            RegisterSqlToDomainObjectsBuilders(services);

            RegisterDataManagers(services);
        }

        private static void RegisterSqlToDomainObjectsBuilders(IServiceCollection services)
        {
            services.AddSingleton<IObjectBuilder<ConfiguredAccountEntity, ConfiguredAccount>, ConfiguredAccountBuilder>();
        }

        private static void RegisterDataManagers(IServiceCollection services)
        {
            services.AddSingleton<IAccountDataManager, AccountDataManager>();
        }
    }
}