using System.Diagnostics.CodeAnalysis;
using FunFair.Common.Data.Builders;
using FunFair.Common.ObjectLocking.Data.Interfaces;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.Builders.ObjectBuilders.Models;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking.DataManagers;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Locking
{
    [ExcludeFromCodeCoverage]
    internal static class ObjectLocking
    {
        public static void Configure(IServiceCollection services)
        {
            RegisterSqlToDomainObjectsBuilders(services);
            RegisterDomainToSqlDataTableBuilders(services);

            RegisterDataManagers(services);
        }

        private static void RegisterSqlToDomainObjectsBuilders(IServiceCollection services)
        {
            // TODO: work out how to simplify, if at all possible
            services.AddSingleton(typeof(IObjectBuilder<ObjectLockEntity<EthereumAddress>, ObjectLock<EthereumAddress>>), typeof(ObjectLockBuilder<EthereumAddress>));
        }

        [SuppressMessage(category: "Microsoft.Usage", checkId: "CA1801:ReviewUnusedParameters", Justification = "Not needed yet, but a placeholder for when it is")]
        [SuppressMessage(category: "ReSharper", checkId: "UnusedParameter.Local", Justification = "Not needed yet, but a placeholder for when it is")]
        private static void RegisterDomainToSqlDataTableBuilders(IServiceCollection services)
        {
            // None here
        }

        private static void RegisterDataManagers(IServiceCollection services)
        {
            services.AddSingleton<IObjectLockDataManager<EthereumAddress>, GameManagerLockDataManager>();
        }
    }
}