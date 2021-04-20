using System;
using FunFair.Common.Environment.Extensions;
using FunFair.Ethereum.Crypto.Interfaces;
using FunFair.Labs.ScalingEthereum.Authentication.Config;
using FunFair.Labs.ScalingEthereum.Authentication.Events;
using FunFair.Labs.ScalingEthereum.Server.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FunFair.Labs.ScalingEthereum.Server.ServiceStartup
{
    /// <summary>
    ///     Authentication setup
    /// </summary>
    internal static class AuthenticationSetup
    {
        public static void Configure(IServiceCollection services, ApplicationConfiguration applicationConfiguration)
        {
            // configure authentication
            IJwtConfig jwtConfig = applicationConfiguration.JwtConfig();

            if (jwtConfig.EcDsaPublicKey == null)
            {
                throw new InvalidOperationException("API requires ECDSA public key to be specified in config");
            }

            // configure authentication
            services.AddSingleton(jwtConfig);

            ServiceProvider sp = services.BuildServiceProvider();

            // Resolve the services from the service provider
            IEcDsaKeyPairLoader ecDsaKeyPairLoader = sp.GetRequiredService<IEcDsaKeyPairLoader>();

            ECDsaSecurityKey ecDsaSecurityKey = ecDsaKeyPairLoader.LoadPublicKey(jwtConfig.EcDsaPublicKey);

            services.AddAuthentication(configureOptions: options =>
                                                         {
                                                             options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                                             options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                                             options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                                                         })
                    .AddJwtBearer(configureOptions: options =>
                                                    {
                                                        options.RequireHttpsMetadata = false;
                                                        options.SaveToken = true;
                                                        options.TokenValidationParameters = new TokenValidationParameters
                                                                                            {
                                                                                                ValidAudience = jwtConfig.Audience,
                                                                                                ValidIssuer = jwtConfig.Issuer,
                                                                                                ValidateIssuerSigningKey = true,
                                                                                                IssuerSigningKey = ecDsaSecurityKey,
                                                                                                ValidateLifetime = true
                                                                                            };
                                                        options.EventsType = typeof(JwtEvents);
                                                    });

            if (applicationConfiguration.Environment.IsLocalDevelopmentOrTest())
            {
                IdentityModelEventSource.ShowPII = true;
            }
        }
    }
}