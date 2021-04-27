using FunFair.Common.Data.Builders;
using FunFair.Common.TypeConverters.Dapper;
using FunFair.Ethereum.TypeConverters.Dapper;
using FunFair.Labs.ScalingEthereum.Data.SqlServer.TypeHandlers;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;

namespace FunFair.Labs.ScalingEthereum.Data.SqlServer
{
    /// <summary>
    ///     Registers Types with dapper.
    /// </summary>
    internal static class SqlTypeMappingRegistry
    {
        /// <summary>
        ///     Registers Types with dapper.
        /// </summary>
        public static void RegisterTypes()
        {
            SqlTypeMapping.RegisterTypes(EthereumTypeHandlerRegistry.Register, CommonTypeHandlerRegistry.Register, RegisterServerSpecificTypes);
        }

        private static void RegisterServerSpecificTypes(ITypeMappingRegistrar handlers)
        {
            handlers.AddHandler<GameRoundId, GameRoundIdHandler>();
            handlers.AddHandler<Seed, SeedHandler>();
            handlers.AddHandler<Token, TokenHandler>();
            handlers.AddHandler<WinLoss, WinLossHandler>();
        }
    }
}