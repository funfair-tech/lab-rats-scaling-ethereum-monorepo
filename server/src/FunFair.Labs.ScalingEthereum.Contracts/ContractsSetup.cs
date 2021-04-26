using System;
using System.Diagnostics.CodeAnalysis;
using FunFair.Ethereum.Abi;
using FunFair.Ethereum.Contracts;
using FunFair.Labs.ScalingEthereum.Contracts.AbiConverters;
using FunFair.Labs.ScalingEthereum.Contracts.Services;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace FunFair.Labs.ScalingEthereum.Contracts
{
    /// <summary>
    ///     Configures the Ethereum Contracts
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ContractsSetup
    {
        /// <summary>
        ///     Configures the Ethereum Contracts
        /// </summary>
        /// <param name="services">The services collection to register services in.</param>
        public static void Configure(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IContractInfoRegistry, ContractInfoRegistry>();

            AddAbiConverters(services);
        }

        private static void AddAbiConverters(IServiceCollection services)
        {
            services.AddSingleton<IAbiType<GameRoundId>, GameRoundIdAbiConverter>();
            services.AddSingleton<IAbiType<WinLoss>, WinLossAbiConverter>();
            services.AddSingleton<IAbiType<Seed>, SeedAbiConverter>();
            services.AddSingleton<IAbiType<DataTypes.Primitives.Token>, TokenAbiConverter>();
        }
    }
}