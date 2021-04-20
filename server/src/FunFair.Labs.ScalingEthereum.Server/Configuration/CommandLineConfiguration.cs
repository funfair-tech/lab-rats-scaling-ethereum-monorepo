using Microsoft.Extensions.Configuration;

namespace FunFair.Labs.ScalingEthereum.Server.Configuration
{
    internal static class CommandLineConfiguration
    {
        public static (int httpPort, int httpsPort, int h2Port) GetServerUrls(IConfigurationRoot config)
        {
            const int defaultHttpPort = 7020;
            const int defaultHttpsPort = 7021;
            const int defaultH2Port = 7022;

            int httpPort = config.GetValue<int?>(key: @"port") ?? defaultHttpPort;
            int httpsPort = config.GetValue<int?>(key: @"https-port") ?? defaultHttpsPort;
            int h2Port = config.GetValue<int?>(key: @"h2-port") ?? defaultH2Port;

            return (httpPort, httpsPort, h2Port);
        }
    }
}