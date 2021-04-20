using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FunFair.Common.Environment.Extensions;
using FunFair.Common.Extensions;
using FunFair.Labs.ScalingEthereum.Server.Configuration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace FunFair.Labs.ScalingEthereum.Server.ServiceStartup
{
    [ExcludeFromCodeCoverage]
    internal static class SignalR
    {
        private static SignalRConnectionFactory? _signalRConnectionFactory;

        public static void AddSignalR(this IServiceCollection services, ApplicationConfiguration applicationConfiguration)
        {
            // register SignalR
            ISignalRServerBuilder signalRBuilder = services.AddSignalR(configure: options => { options.EnableDetailedErrors = applicationConfiguration.Environment.IsLocalDevelopmentOrTest(); })
                                                           .AddJsonProtocol(configure: options =>
                                                                                       {
                                                                                           JsonSerializerOptions serializerSettings = options.PayloadSerializerOptions;
                                                                                           serializerSettings.IgnoreNullValues = true;
                                                                                           serializerSettings.PropertyNameCaseInsensitive = false;
                                                                                           serializerSettings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                                                                                           serializerSettings.WriteIndented = false;

                                                                                           JsonConverterSetup.Configure(serializerSettings.Converters);

                                                                                           // Note Additional converters that require DI are enabled elsewhere
                                                                                       });

            // if we're running anywhere but LOCAL or TEST then configure the redis backplane for SignalR
            // DON'T ATTEMPT TO ENABLE THIS LOCALLY, IT WILL NOT WORK. AWS redis does not allow connections
            // from outside AWS (regardless of security group configuration)
            if (!applicationConfiguration.Environment.IsLocalOrTest())
            {
                _signalRConnectionFactory = new SignalRConnectionFactory();

                signalRBuilder.AddStackExchangeRedis(configure: options =>
                                                                {
                                                                    options.Configuration.ChannelPrefix = $"Labs-NFT-Server-{applicationConfiguration.Environment.GetName()}";
                                                                    options.ConnectionFactory =
                                                                        writer => _signalRConnectionFactory.ConnectionFactoryAsync(writer: writer, applicationConfiguration: applicationConfiguration);
                                                                });
            }
        }

        public static void EnableLogging(ILoggerFactory loggerFactory)
        {
            _signalRConnectionFactory?.EnableLogging(loggerFactory.CreateLogger<SignalRConnectionFactory>());
        }

        private sealed class SignalRConnectionFactory
        {
            private ILogger<SignalRConnectionFactory>? _logger;

            public SignalRConnectionFactory()
            {
                this._logger = null;
            }

            public void EnableLogging(ILogger<SignalRConnectionFactory> logger)
            {
                this._logger = logger;
            }

            public async Task<IConnectionMultiplexer> ConnectionFactoryAsync(TextWriter writer, ApplicationConfiguration applicationConfiguration)
            {
                ConfigurationOptions config = new() {AbortOnConnectFail = false};

                config.EndPoints.Add(host: applicationConfiguration.RedisHost, port: applicationConfiguration.RedisPort);
                ConnectionMultiplexer connection = await ConnectionMultiplexer.ConnectAsync(configuration: config, log: writer);

                connection.ConnectionFailed += (_, eventArgs) => this.LogConnectionFailed(eventArgs);
                connection.ConnectionRestored += (_, eventArgs) => this.LogConnectionRestored(eventArgs);

                if (!connection.IsConnected)
                {
                    this.LogDidNotConnect();
                }

                return connection;
            }

            private void LogDidNotConnect()
            {
                this._logger?.LogError("Did not connect to Redis");
            }

            private void LogConnectionRestored(ConnectionFailedEventArgs connectionFailedEventArgs)
            {
                if (connectionFailedEventArgs.Exception != null)
                {
                    this._logger?.LogInformation(new EventId(connectionFailedEventArgs.Exception.HResult),
                                                 exception: connectionFailedEventArgs.Exception,
                                                 $"Connection to Redis restored:  {connectionFailedEventArgs.Exception.Message}");
                }
                else
                {
                    this._logger?.LogInformation($"Connection to Redis restored. {connectionFailedEventArgs.FailureType.GetName()}");
                }
            }

            private void LogConnectionFailed(ConnectionFailedEventArgs connectionFailedEventArgs)
            {
                if (connectionFailedEventArgs.Exception != null)
                {
                    this._logger?.LogError(new EventId(connectionFailedEventArgs.Exception.HResult),
                                           exception: connectionFailedEventArgs.Exception,
                                           $"Connection to Redis failed:  {connectionFailedEventArgs.Exception.Message}");
                }
                else
                {
                    this._logger?.LogError($"Connection to Redis failed. {connectionFailedEventArgs.FailureType.GetName()}");
                }
            }
        }
    }
}