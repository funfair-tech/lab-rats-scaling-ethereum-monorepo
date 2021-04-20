using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FunFair.Labs.ScalingEthereum.Server.Configuration;
using FunFair.Labs.ScalingEthereum.Server.ServiceStartup;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FunFair.Labs.ScalingEthereum.Server
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine(value: "Starting...");
            AppContext.SetSwitch(switchName: "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", isEnabled: true);

            IConfigurationRoot commandLineConfiguration = new ConfigurationBuilder().AddCommandLine(args)
                                                                                    .Build();

            (int httpPort, int httpsPort, int h2Port) = CommandLineConfiguration.GetServerUrls(commandLineConfiguration);

            using (IWebHost host = CreateWebHost(args: args, httpPort: httpPort, httpsPort: httpsPort, h2Port: h2Port))
            {
                await ApplicationSetup.StartupAsync(serviceProvider: host.Services);

                await host.RunAsync();

                await ApplicationSetup.ShutdownAsync();
            }
        }

        private static IWebHost CreateWebHost(string[] args, int httpPort, int httpsPort, int h2Port)
        {
            AppContext.SetSwitch(switchName: "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", isEnabled: true);

            return WebHost.CreateDefaultBuilder(args)
                          .UseKestrel(options: options => SetKestrelOptions(options: options, httpPort: httpPort, httpsPort: httpsPort, h2Port: h2Port))
                          .UseSetting(key: WebHostDefaults.SuppressStatusMessagesKey, value: "True")
                          .UseStartup<Startup>()
                          .ConfigureLogging((_, logger) => logger.ClearProviders())
                          .UseSockets(SetSocketOptions)
                          .Build();
        }

        private static void SetSocketOptions(SocketTransportOptions socketTransportOptions)
        {
            Console.WriteLine($"Using Sockets with {socketTransportOptions.IOQueueCount} threads");
        }

        private static void SetH2ListenOptions(ListenOptions listenOptions)
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        }

        private static void SetHttpsListenOptions(ListenOptions listenOptions)
        {
            string certFile = Path.Combine(path1: ApplicationConfigLocator.ConfigurationFilesPath, path2: "server.pfx");

            if (!File.Exists(certFile))
            {
                listenOptions.Protocols = HttpProtocols.Http1;
                listenOptions.UseHttps();
            }
            else
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                listenOptions.UseHttps(fileName: certFile);
            }
        }

        private static void SetKestrelOptions(KestrelServerOptions options, int httpPort, int httpsPort, int h2Port)
        {
            options.Limits.MaxConcurrentConnections = 100;
            options.Limits.MaxConcurrentUpgradedConnections = 100;
            options.DisableStringReuse = false;
            options.AllowSynchronousIO = true;

            options.AddServerHeader = false;
            options.Limits.MinResponseDataRate = null;
            options.Limits.MinRequestBodyDataRate = null;

            if (httpsPort != 0)
            {
                options.Listen(address: IPAddress.Any, port: httpsPort, configure: SetHttpsListenOptions);
            }

            if (h2Port != 0)
            {
                options.Listen(address: IPAddress.Any, port: h2Port, configure: SetH2ListenOptions);
            }

            options.Listen(address: IPAddress.Any, port: httpPort);
        }
    }
}