using System.Diagnostics.CodeAnalysis;
using FunFair.Common.Data.Builders;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.GameRound;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.DataTableBuilders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.DataTableBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.Builders.ObjectBuilders.Entities;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Games.DataManagers;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Games
{
    [ExcludeFromCodeCoverage]
    internal static class GameSupport
    {
        public static void Configure(IServiceCollection services)
        {
            RegisterSqlToDomainObjectsBuilders(services);
            RegisterDomainToSqlDataTableBuilders(services);

            RegisterDataManagers(services);
        }

        private static void RegisterSqlToDomainObjectsBuilders(IServiceCollection services)
        {
            services.AddSingleton<IObjectBuilder<BlockNumberEntity, BlockNumber>, BlockNumberBuilder>();
            services.AddSingleton<IObjectBuilder<GameRoundEntity, GameRound>, GameRoundBuilder>();
            services.AddSingleton<IObjectBuilder<GameRoundPlayerWinEntity, GameRoundPlayerWin>, GameRoundPlayerWinBuilder>();
            services.AddSingleton<IObjectBuilder<GameHistoryEntity, GameHistory>, GameHistoryBuilder>();
            services.AddSingleton<IObjectBuilder<TransactionHashEntity, TransactionHash>, TransactionHashBuilder>();
        }

        private static void RegisterDomainToSqlDataTableBuilders(IServiceCollection services)
        {
            services.AddSingleton<ISqlDataTableBuilder<WinAmountEntity>, WinAmountEntityDataTableBuilder>();
        }

        private static void RegisterDataManagers(IServiceCollection services)
        {
            services.AddSingleton<IGameRoundDataManager, GameRoundDataManager>();
        }
    }
}