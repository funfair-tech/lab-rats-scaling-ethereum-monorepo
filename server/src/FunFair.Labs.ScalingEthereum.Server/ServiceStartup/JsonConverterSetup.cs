using System.Collections.Generic;
using System.Text.Json.Serialization;
using FunFair.Ethereum.TypeConverters.Json;
using FunFair.Labs.ScalingEthereum.TypeConverters.Json;

namespace FunFair.Labs.ScalingEthereum.Server.ServiceStartup
{
    /// <summary>
    ///     Configures the Json converters
    /// </summary>
    public static class JsonConverterSetup
    {
        /// <summary>
        ///     Configures the Json converters
        /// </summary>
        /// <param name="converters">Registry of converters.</param>
        public static void Configure(IList<JsonConverter> converters)
        {
            converters.Add(new AccountAddressConverter());
            converters.Add(new BlockNumberConverter());
            converters.Add(new EthereumAmountConverter());
            converters.Add(new HexAddressConverter());
            converters.Add(new SeedConverter());
            converters.Add(new TransactionHashConverter());
            converters.Add(new TokenConverter());
        }
    }
}