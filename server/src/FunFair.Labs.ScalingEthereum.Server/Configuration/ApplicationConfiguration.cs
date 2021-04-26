using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using FunFair.Common.Data;
using FunFair.Common.Data.Services;
using FunFair.Common.Environment;
using FunFair.Common.Extensions;
using FunFair.Content.Interfaces;
using FunFair.Ethereum.DataTypes;
using FunFair.Ethereum.DataTypes.Helpers;
using FunFair.Ethereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Authentication.Config;
using FunFair.Labs.ScalingEthereum.DataTypes.Primitives;
using FunFair.Labs.ScalingEthereum.Logic.Faucet.Models;
using FunFair.Labs.ScalingEthereum.Logic.House;
using FunFair.Labs.ScalingEthereum.Logic.House.Models.FunFair.Labs.MultiPlayer.Logic.Alerts.Models;
using FunFair.Labs.ScalingEthereum.Server.ServiceStartup;
using Microsoft.Extensions.Configuration;

namespace FunFair.Labs.ScalingEthereum.Server.Configuration
{
    internal sealed class ApplicationConfiguration
    {
        public ApplicationConfiguration(IConfigurationRoot config)
        {
            this.Configuration = config ?? throw new ArgumentNullException(nameof(config));
            this.Version = ServerVersion()
                .ToString();
        }

        public string Version { get; }

        public IConfigurationRoot Configuration { get; }

        public ExecutionEnvironment Environment =>
            this.Configuration[key: @"Environment"]
                .ToEnum<ExecutionEnvironment>();

        public IFaucetConfiguration FaucetConfiguration { get; } = new FaucetConfiguration(tokenToGive: new Token(value: 100_000m), ethToGive: new EthereumAmount(0.1m.ToWei(EthereumUnit.ETHER)));

        public IFaucetBalanceConfiguration FaucetBalanceConfiguration
        {
            get
            {
                EthereumAmount minEthBalance = new(decimal.Parse(this.Configuration[key: @"Alerts:FaucetBalances:MinimumEth"] ?? "1", provider: CultureInfo.InvariantCulture)
                                                          .ToWei(EthereumUnit.ETHER));
                Token minTokenBalance = new(decimal.Parse(this.Configuration[key: @"Alerts:FaucetBalances:MinimumToken"] ?? "1000.0", provider: CultureInfo.InvariantCulture));

                return new FaucetBalanceConfiguration(minimumAllowedNativeCurrencyBalance: minEthBalance, minimumAllowedTokenBalance: minTokenBalance.Erc20Value.TokenAmount);
            }
        }

        public IHouseBalanceConfiguration HouseBalanceConfiguration
        {
            get
            {
                EthereumAmount minEthBalance = new(decimal.Parse(this.Configuration[key: @"Alerts:HouseBalances:MinimumEth"] ?? "1", provider: CultureInfo.InvariantCulture)
                                                          .ToWei(EthereumUnit.ETHER));

                return new HouseBalanceConfiguration(minimumAllowedBalance: minEthBalance);
            }
        }

        public ISqlServerConfiguration SqlServerConfiguration
        {
            get
            {
                string connectionString = this.Configuration[key: "DatabaseConfiguration:ConnectionString"];

                if (this.Environment == ExecutionEnvironment.LOCAL)
                {
                    SqlConnectionStringBuilder builder = new(connectionString);

                    if (!string.IsNullOrWhiteSpace(builder.AttachDBFilename) && builder.DataSource.StartsWith(value: "(localdb)\\", comparisonType: StringComparison.OrdinalIgnoreCase))
                    {
                        const string replacement = "|DataDirectory|";

                        if (builder.AttachDBFilename.StartsWith(value: replacement, comparisonType: StringComparison.OrdinalIgnoreCase))
                        {
                            builder.AttachDBFilename = Path.Join(path1: ApplicationConfigLocator.ConfigurationFilesPath, builder.AttachDBFilename.Substring(replacement.Length));
                            connectionString = builder.ConnectionString;
                        }
                    }
                }

                return new SqlServerConfiguration(connectionString);
            }
        }

        public string RedisHost => this.Configuration[key: "RedisConfiguration:Host"];

        public int RedisPort => this.Configuration.GetValue<int>(key: "RedisConfiguration:Port");

        public string WalletSalt => this.Configuration.GetValue<string>(key: @"Accounts:Salt");

        public Uri Proxy => new(this.Configuration[key: "Ethereum:Proxy"]);

        public Uri Balances => new(this.Configuration[key: "Ethereum:Balances"]);

        public LoggingConfiguration LoggingConfiguration => new() {LogglyToken = this.Configuration[key: "Logging:Loggly:Token"]};

        public string Tenant => this.Configuration[key: @"Tenant"];

        public IJwtConfig JwtConfig()
        {
            return new JwtConfig
                   {
                       EcDsaPublicKey =
                           string.IsNullOrEmpty(this.Configuration[key: "JwtConfig:EcDsaPublicKey"]) ? null : Convert.FromBase64String(this.Configuration[key: "JwtConfig:EcDsaPublicKey"]),
                       Audience = this.Configuration[key: "JwtConfig:Issuer"],
                       Issuer = this.Configuration[key: "JwtConfig:Audience"]
                   };
        }

        private static PackageVersion ServerVersion()
        {
            string location = GetApplication();

            Console.WriteLine($"App Location: {location}");

            if (string.IsNullOrWhiteSpace(location))
            {
#if NORMAL
                throw new NotSupportedException(message: "Unsupported Assembly version (Could not find Application)");
#else
                return new PackageVersion("1.2.3.4-no-application");
#endif
            }

            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(location);

            string? assemblyVersion = fileVersionInfo.ProductVersion;

            if (string.IsNullOrWhiteSpace(assemblyVersion))
            {
#if NORMAL
                throw new NotSupportedException(message: "Unsupported Assembly version (Missing)");
#else
                Version? executingAssembly = Assembly.GetExecutingAssembly()
                                                     .GetName()
                                                     .Version;

                if (executingAssembly != null)
                {
                    return new PackageVersion(executingAssembly.ToString());
                }

                return new PackageVersion("1.2.3.4-no-file-version");
#endif
            }

            return new PackageVersion(assemblyVersion);
        }

        private static string GetApplication()
        {
#if SINGLE_FILE_PUBLISH
            // note for bundled applications Assembly Location returns NULL so have to use CommandLineArgs to get the app name.
            return System.Environment.GetCommandLineArgs()[0];
#else
            return typeof(ApplicationConfiguration).Assembly.Location;
#endif
        }
    }
}