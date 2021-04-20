using System.Diagnostics.CodeAnalysis;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.PlayerCount;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Players.DataManagers;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Players
{
    [ExcludeFromCodeCoverage]
    internal static class Player
    {
        public static void Configure(IServiceCollection services)
        {
            RegisterDataManagers(services);
        }

        private static void RegisterDataManagers(IServiceCollection services)
        {
            services.AddSingleton<IPlayerCountDataManager, PlayerCountDataManager>();
        }
    }
}