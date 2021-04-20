using System.Diagnostics.CodeAnalysis;
using FunFair.Common.Data.Builders;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Ethereum.Events.Data.Interfaces;
using FunFair.Ethereum.Events.Data.Interfaces.Models;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.DataTableBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.ObjectBuilders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.Builders.ObjectBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Events.DataManagers;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Events
{
    [ExcludeFromCodeCoverage]
    internal static class EventTransactions
    {
        public static void Configure(IServiceCollection services)
        {
            RegisterSqlToDomainObjectsBuilders(services);
            RegisterDomainToSqlDataTableBuilders(services);

            RegisterDataManagers(services);
        }

        private static void RegisterSqlToDomainObjectsBuilders(IServiceCollection services)
        {
            services.AddSingleton<IObjectBuilder<AwaitingConfirmationsTransactionEntity, AwaitingConfirmationsTransaction>, AwaitingConfirmationsTransactionBuilder>();
            services.AddSingleton<IObjectBuilder<EventContractCheckpointEntity, EventContractCheckpoint>, EventContractCheckpointBuilder>();
            services.AddSingleton<IObjectBuilder<EventTransactionHashEntity, TransactionHash>, TransactionHashBuilder>();
        }

        private static void RegisterDomainToSqlDataTableBuilders(IServiceCollection services)
        {
            services.AddSingleton<ISqlDataTableBuilder<EventTransactionEntity>, EventTransactionHashDataTableBuilder>();
            services.AddSingleton<ISqlDataTableBuilder<ContractAddressEntity>, ContractAddressEntityDataTableBuilder>();
            services.AddSingleton<ISqlDataTableBuilder<AwaitingConfirmationsTransactionDataTableEntity>, AwaitingConfirmationsTransactionDataTableEntityDataTableBuilder>();
            services.AddSingleton<ISqlDataTableBuilder<EventIndexEntity>, EventIndexEntityDataTableBuilder>();
        }

        private static void RegisterDataManagers(IServiceCollection services)
        {
            services.AddSingleton<IEventDataManager, EventDataManager>();
        }
    }
}