using System.Diagnostics.CodeAnalysis;
using FunFair.Labs.ScalingEthereum.Data.Interfaces.Faucet;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.Faucet.DataManagers;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer.Faucet
{
    [ExcludeFromCodeCoverage]
    internal static class FaucetTracking
    {
        public static void Configure(IServiceCollection services)
        {
            RegisterDataManagers(services);
        }

        private static void RegisterDataManagers(IServiceCollection services)
        {
            services.AddSingleton<IFaucetDataManager, FaucetDataManager>();
        }
    }
}