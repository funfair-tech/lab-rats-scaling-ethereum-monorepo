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

            RegisterDataManagers(services);
        }

        private static void RegisterSqlToDomainObjectsBuilders(IServiceCollection services)
        {
            services.AddSingleton(typeof(IObjectBuilder<ObjectLockEntity<EthereumAddress>, ObjectLock<EthereumAddress>>), typeof(ObjectLockBuilder<EthereumAddress>));
        }

        private static void RegisterDataManagers(IServiceCollection services)
        {
            services.AddSingleton<IObjectLockDataManager<EthereumAddress>, GameManagerLockDataManager>();
        }
    }
}