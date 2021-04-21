using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using FluentValidation.AspNetCore;
using FunFair.Common.Middleware;
using FunFair.Common.Middleware.Helpers;
using FunFair.Ethereum.Crypto.Interfaces;
using FunFair.Ethereum.Crypto.Services;
using FunFair.Ethereum.Networks.Interfaces;
using FunFair.Ethereum.TypeConverters.Json;
using FunFair.Labs.ScalingEthereum.Authentication.Events;
using FunFair.Labs.ScalingEthereum.DataTypes.Wallet;
using FunFair.Labs.ScalingEthereum.Server.Configuration;
using FunFair.Labs.ScalingEthereum.Server.Middleware;
using FunFair.Labs.ScalingEthereum.Server.ServiceStartup;
using FunFair.Labs.ScalingEthereum.ServiceInterface.Hub;
using FunFair.Labs.ScalingEthereum.ServiceInterfaces.Binders;
using FunFair.Labs.ScalingEthereum.ServiceInterfaces.ModelValidation.Faucet;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NWebsec.Core.Common.Middleware.Options;
using Serilog;

namespace FunFair.Labs.ScalingEthereum.Server
{
    /// <summary>
    ///     Starts the application.
    /// </summary>
    public sealed class Startup
    {
        private readonly ApplicationConfiguration _configuration;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public Startup(IHostEnvironment env)
        {
            env.ContentRootFileProvider = new NullFileProvider();

            string configPath = ApplicationConfigLocator.ConfigurationFilesPath;

            IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(configPath)
                                                                      .AddJsonFile(path: "appsettings.json", optional: true, reloadOnChange: true)
                                                                      .AddJsonFile(path: "appsettings-local.json", optional: true, reloadOnChange: true)
                                                                      .AddEnvironmentVariables();

            this._configuration = new ApplicationConfiguration(builder.Build());
        }

        /// <summary>
        ///     Configures the services.
        /// </summary>
        /// <param name="services">the services to register.</param>
        /// <remarks>This method gets called by the runtime. Use this method to add services to the container.</remarks>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Called by runtime")]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache(x => x.SizeLimit = 1024 * 1024 * 8)
                    .AddCors()
                    .AddControllers();

            services.AddSingleton<ValidationActionFilter>();

            services.Configure<GzipCompressionProviderOptions>(configureOptions: options => options.Level = CompressionLevel.Fastest)
                    .Configure<BrotliCompressionProviderOptions>(configureOptions: options => options.Level = CompressionLevel.Fastest)
                    .AddResponseCompression(configureOptions: options =>
                                                              {
                                                                  options.EnableForHttps = true;

                                                                  // Explicitly enable Gzip
                                                                  options.Providers.Add<BrotliCompressionProvider>();
                                                                  options.Providers.Add<GzipCompressionProvider>();

                                                                  // Add Custom mime types
                                                                  options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {"image/svg+xml"});
                                                              });

            Logging.ConfigureLogging(services: services, environment: this._configuration.Environment);

            services.AddSignalR(this._configuration);

            ApplicationSetup.Configure(services: services, applicationConfiguration: this._configuration);

            // configure authentication
            services.AddSingleton<IEcDsaKeyPairLoader, EcDsaKeyPairLoader>();
            services.AddSingleton<JwtEvents>();

            AuthenticationSetup.Configure(services: services, applicationConfiguration: this._configuration);

            services.AddResponseCaching()
                    .AddMvc(setupAction: options => options.Filters.AddService(typeof(ValidationActionFilter)))
                    .AddMvcOptions(setupAction: _ =>
                                                {
                                                    // Note Additional ModelMetadata providers that require DI are enabled elsewhere
                                                })
                    .SetCompatibilityVersion(version: CompatibilityVersion.Latest)
                    .AddDataAnnotationsLocalization()
                    .AddFluentValidation(configurationExpression: v => v.RegisterValidatorsFromAssemblyContaining<OpenFaucetDtoValidator>())
                    .AddJsonOptions(configure: options =>
                                               {
                                                   JsonSerializerOptions serializerSettings = options.JsonSerializerOptions;
                                                   serializerSettings.IgnoreNullValues = true;
                                                   serializerSettings.PropertyNameCaseInsensitive = false;
                                                   serializerSettings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                                                   serializerSettings.WriteIndented = false;

                                                   JsonConverterSetup.Configure(serializerSettings.Converters);
                                               });

            services.ConfigureSwaggerServices(version: this._configuration.Version)
                    .AddRouting();
        }

        /// <summary>
        ///     Configures the HTTP request pipeline
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="applicationLifeTime">The lifetime of the application.</param>
        /// <param name="loggerFactory">Logger factory</param>
        /// <remarks>This method gets called by the runtime. Use this method to configure the HTTP request pipeline.</remarks>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Called by runtime")]
        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifeTime, ILoggerFactory loggerFactory)
        {
            RegisterEthereumNetworkConverter(app.ApplicationServices);
            RegisterAdditionalMvcOptions(app.ApplicationServices);
            RegisterSignalRJsonSerializerOptions(app.ApplicationServices);

            IOptions<WalletConfiguration> walletConfiguration = app.ApplicationServices.GetRequiredService<IOptions<WalletConfiguration>>();

            const string dataUrl = @"data:";
            const string googleFonts = @"https://fonts.googleapis.com";
            const string googleFontsStatic = @"https://fonts.gstatic.com";

            string walletSdk = new Uri(walletConfiguration.Value.SdkUri.AbsoluteUri).BuildOriginUrl()
                                                                                    .ToString();

            string[] imageSources =
            {
                dataUrl,

                // Wallet
                walletSdk
            };

            string[] frameSources = {walletSdk};

            void CspOptions(IFluentCspOptions options)
            {
                options.ImageSources(x => x.Self())
                       .ImageSources(x => x.CustomSources(imageSources))
                       .StyleSources(configurer: s => s.Self())
                       .StyleSources(configurer: s => s.UnsafeInline())
                       .StyleSources(configurer: s => s.CustomSources(googleFonts, walletSdk))
                       .FontSources(configurer: s => s.Self())
                       .FontSources(configurer: s => s.Self())
                       .FontSources(configurer: s => s.CustomSources(dataUrl, googleFontsStatic))
                       .MediaSources(configurer: s => s.Self())
                       .FrameSources(configurer: s => s.CustomSources(frameSources))
                       .FrameSources(configurer: s => s.Self())
                       .ScriptSources(configurer: s => s.Self())
                       .ScriptSources(configurer: s => s.UnsafeInline())
                       .ScriptSources(configurer: s => s.UnsafeEval())
                       .ScriptSources(configurer: s => s.CustomSources(dataUrl, walletSdk))
                       .ConnectSources(x => x.Self())
                       .ConnectSources(x => x.CustomSources(@"wss://host"));
            }

            // note that the order of the branches is important. If a branch does not serve the correct files then it could be due to the ordering.
            app.UseRequestIpAddressMiddleware()
               .UseDefaultToNoResponseCachingMiddleware()
               .UseResponseCompression()
               .UseErrorHandling()
               .UseHstsHeader()
               .UseServerNameHeader()
               .UseForwardedHeaders(new ForwardedHeadersOptions {ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost})
               .RegisterSecurityHeaders(cspOptionsBuilder: CspOptions, applicationConfiguration: this._configuration)
               .UseMiddleware<CspWssHostnameMiddleware>()
               .UseRouting()
               .UseAuthentication()
               .UseAuthorization()
               .UseEndpoints(configure: endpoints =>
                                        {
                                            endpoints.MapControllers();
                                            endpoints.MapHub<AuthenticatedHub>(pattern: "hub/authenticated");
                                            endpoints.MapHub<PublicHub>(pattern: "hub/public");
                                        });

            ServiceStartup.Swagger.RegisterSwagger(app: app, version: this._configuration.Version);

            // Ensure any buffered events are sent at shutdown
            applicationLifeTime.ApplicationStopping.Register(Log.CloseAndFlush);

            DisableTelemetry(app);

            SignalR.EnableLogging(loggerFactory);
        }

        private static void DisableTelemetry(IApplicationBuilder app)
        {
            // Ensure that telemetry is disabled! No real idea why this ends up turned on in some cases but not in others.
            TelemetryConfiguration? telemetryConfiguration = app.ApplicationServices.GetService<TelemetryConfiguration>();

            if (telemetryConfiguration != null)
            {
                telemetryConfiguration.DisableTelemetry = true;
            }
        }

        private static void RegisterEthereumNetworkConverter(IServiceProvider serviceProvider)
        {
            IEthereumNetworkRegistry networkRegistry = serviceProvider.GetRequiredService<IEthereumNetworkRegistry>();

            IOptions<JsonOptions> mvcJsonOptions = serviceProvider.GetRequiredService<IOptions<JsonOptions>>();

            EthereumNetworkConverter ethereumNetworkConverter = new(networkRegistry);

            mvcJsonOptions.Value.JsonSerializerOptions.Converters.Add(ethereumNetworkConverter);
        }

        private static void RegisterAdditionalMvcOptions(IServiceProvider serviceProvider)
        {
            IOptions<MvcOptions> mvcOptions = serviceProvider.GetRequiredService<IOptions<MvcOptions>>();

            mvcOptions.Value.ModelBinderProviders.Insert(index: 0, new ModelMetadataDetailsProvider(serviceProvider));
        }

        private static void RegisterSignalRJsonSerializerOptions(IServiceProvider serviceProvider)
        {
            IEthereumNetworkRegistry networkRegistry = serviceProvider.GetRequiredService<IEthereumNetworkRegistry>();

            IOptions<JsonOptions> mvcJsonOptions = serviceProvider.GetRequiredService<IOptions<JsonOptions>>();
            IOptions<JsonHubProtocolOptions> signalrJsonOptions = serviceProvider.GetRequiredService<IOptions<JsonHubProtocolOptions>>();

            EthereumNetworkConverter ethereumNetworkConverter = new(networkRegistry);

            mvcJsonOptions.Value.JsonSerializerOptions.Converters.Add(ethereumNetworkConverter);
            signalrJsonOptions.Value.PayloadSerializerOptions.Converters.Add(ethereumNetworkConverter);
        }
    }
}