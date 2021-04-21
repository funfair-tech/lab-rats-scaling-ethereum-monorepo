using System.Diagnostics.CodeAnalysis;
using FunFair.Common.Environment;
using FunFair.Common.Environment.Extensions;
using FunFair.Common.Extensions;
using FunFair.Labs.ScalingEthereum.Server.Configuration;
using Loggly.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace FunFair.Labs.ScalingEthereum.Server.ServiceStartup
{
    internal static class Logging
    {
        private static LoggingLevelSwitch? _loggingLevelSwitch;

        public static void ConfigureLogging(IServiceCollection services, ExecutionEnvironment environment)
        {
            // set up our logging level switch, so we can change the log level on the fly - default to the configured
            // log level
            _loggingLevelSwitch = new LoggingLevelSwitch(environment == ExecutionEnvironment.LOCAL ? LogEventLevel.Debug : LogEventLevel.Warning);
            services.AddSingleton(_loggingLevelSwitch);

            // add logging to the services
            services.AddLogging(AddFilters);
        }

        private static void AddFilters(ILoggingBuilder builder)
        {
            builder.AddFilter(category: @"Microsoft", level: LogLevel.Warning)
                   .AddFilter(category: @"System.Net.Http.HttpClient", level: LogLevel.Warning)
                   .AddFilter(category: @"Microsoft.AspNetCore.ResponseCaching.ResponseCachingMiddleware", level: LogLevel.Error);
        }

        [SuppressMessage(category: "Microsoft.Reliability", checkId: "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Lives for program lifetime")]
        public static void InitializeLogging(ExecutionEnvironment environment, ILoggerFactory loggerFactory, LoggingConfiguration configuration, string version, string tenant)
        {
            ConfigureLoggly(environment: environment, configuration: configuration);

            // set up Serilog logger
            Log.Logger = CreateLogger(environment: environment, version: version, tenant: tenant);

            // set up the logger factory
            loggerFactory.AddSerilog();
        }

        private static void ConfigureLoggly(ExecutionEnvironment environment, LoggingConfiguration configuration)
        {
            if (environment.IsLocalOrTest())
            {
                return;
            }

            ILogglyConfig config = LogglyConfig.Instance;
            config.CustomerToken = configuration.LogglyToken;
            config.Transport.EndpointHostname = "logs-01.loggly.com";
            config.Transport.EndpointPort = 443;
            config.Transport.LogTransport = LogTransport.Https;
        }

        private static Logger CreateLogger(ExecutionEnvironment environment, string version, string tenant)
        {
            const string processName = @"FunFair.Labs.MultiPlayer.Server";

            LoggerConfiguration configuration = new LoggerConfiguration().MinimumLevel.ControlledBy(_loggingLevelSwitch)
                                                                         .Enrich.FromLogContext()
                                                                         .Enrich.WithMachineName()
                                                                         .Enrich.WithProcessId()
                                                                         .Enrich.WithThreadId()
                                                                         .Enrich.WithProperty(name: @"Environment", environment.GetName())
                                                                         .Enrich.WithProperty(name: @"ServerVersion", value: version)
                                                                         .Enrich.WithProperty(name: @"ProcessName", value: processName)
                                                                         .Enrich.WithProperty(name: @"Tenant", value: tenant)
                                                                         .WriteTo.Console();

            if (!environment.IsLocalOrTest())
            {
                configuration = configuration.WriteTo.Loggly();
            }

            return configuration.CreateLogger();
        }
    }
}