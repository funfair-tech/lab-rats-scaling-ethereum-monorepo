using System.Diagnostics.CodeAnalysis;
using FunFair.Common.Data.Builders;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.Transactions.Data.Interfaces;
using FunFair.Ethereum.Transactions.Data.Interfaces.Models;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.DataTableBuilders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.DataTableBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.Builders.ObjectBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions.DataManagers;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Transactions
{
    [ExcludeFromCodeCoverage]
    internal static class SendTransactions
    {
        public static void Configure(IServiceCollection services)
        {
            RegisterSqlToDomainObjectsBuilders(services);
            RegisterDomainToSqlDataTableBuilders(services);

            RegisterDataManagers(services);
        }

        private static void RegisterSqlToDomainObjectsBuilders(IServiceCollection services)
        {
            services.AddSingleton<IObjectBuilder<BrokenTransactionEntity, BrokenTransaction>, BrokenTransactionBuilder>();
            services.AddSingleton<IObjectBuilder<PendingTransactionEntity, PendingTransaction>, PendingTransactionBuilder>();
            services.AddSingleton<IObjectBuilder<TransactionSubmissionStatusEntity, TransactionSubmissionStatus>, TransactionSubmissionStatusBuilder>();
        }

        private static void RegisterDomainToSqlDataTableBuilders(IServiceCollection services)
        {
            services.AddSingleton<ISqlDataTableBuilder<NetworkTransactionReceiptEntity>, NetworkTransactionReceiptEntityDataTableBuilder>();
            services.AddSingleton<ISqlDataTableBuilder<PendingTransaction>, PendingTransactionDataTableBuilder>();
        }

        private static void RegisterDataManagers(IServiceCollection services)
        {
            services.AddSingleton<IEthereumAccountNonceDataManager, EthereumAccountNonceDataManager>();
            services.AddSingleton<IPendingTransactionDataManager, PendingTransactionDataManager>();
            services.AddSingleton<ITransactionDataManager, TransactionDataManager>();
            services.AddSingleton<ITransactionStatusDataManager, TransactionStatusDataManager>();
        }
    }
}