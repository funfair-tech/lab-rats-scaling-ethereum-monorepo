using System;
using FunFair.Common.Environment;
using FunFair.Common.Environment.Extensions;
using FunFair.Common.Middleware;
using FunFair.Labs.ScalingEthereum.Server.Configuration;
using Microsoft.AspNetCore.Builder;
using NWebsec.Core.Common.Middleware.Options;

namespace FunFair.Labs.ScalingEthereum.Server.ServiceStartup
{
    internal static class SecurityHeaders
    {
        public static IApplicationBuilder RegisterSecurityHeaders(this IApplicationBuilder applicationBuilder,
                                                                  Action<IFluentCspOptions> cspOptionsBuilder,
                                                                  ApplicationConfiguration applicationConfiguration)
        {
            static void SetXssOptions(IFluentXXssProtectionOptions options)
            {
                options.EnabledWithBlockMode();
            }

            static void SetRefererPolicy(IFluentReferrerPolicyOptions opts)
            {
                opts.NoReferrerWhenDowngrade(); // required for Tawk.To widget - if set to same origin, this feature will silently fail.
            }

            // we need list of origins till we decide how we will ship monitoring app
            string[] corsOrigins = {"https://localhost:3000", "https://*.netlify.app"};

            return applicationBuilder.UseXFrameOptions(applicationConfiguration)
                                     .UseXContentTypeOptions()
                                     .UseXDownloadOptions()
                                     .UseReferrerPolicy(SetRefererPolicy)
                                     .UseXXssProtection(SetXssOptions)
                                     .UseCors(configurePolicy: options => options.WithOrigins(corsOrigins)
                                                                                 .AllowAnyMethod()
                                                                                 .AllowAnyHeader()
                                                                                 .WithExposedHeaders("Retry-After")
                                                                                 .AllowCredentials()
                                                                                 .SetPreflightMaxAge(TimeSpan.FromMinutes(value: 5)))
                                     .UseContentSecurityPolicy(cspOptionsBuilder, applicationConfiguration.Environment)
                                     .UseExpectCtHeader(applicationConfiguration.Environment);
        }

        private static IApplicationBuilder UseXFrameOptions(this IApplicationBuilder applicationBuilder, ApplicationConfiguration applicationConfiguration)
        {
            if (!applicationConfiguration.Environment.IsLocalDevelopmentOrTest())
            {
                static void SetXfoOptions(IFluentXFrameOptions options)
                {
                    options.SameOrigin();
                }

                return applicationBuilder.UseXfo(SetXfoOptions);
            }

            return applicationBuilder;
        }

        private static void SetCspOptions(IFluentCspOptions options, Action<IFluentCspOptions> cspOptionsBuilder, ExecutionEnvironment executionEnvironment)
        {
            options.BlockAllMixedContent()
                   .DefaultSources(configurer: s => s.None());

            if (ShouldLogToReportUri(executionEnvironment))
            {
                options.ReportUris(configurer: s => s.Uris("https://funfair.report-uri.com/r/d/csp/enforce"));
            }

            cspOptionsBuilder(options);
        }

        private static IApplicationBuilder UseContentSecurityPolicy(this IApplicationBuilder applicationBuilder, Action<IFluentCspOptions> cspOptionsBuilder, ExecutionEnvironment executionEnvironment)
        {
            return applicationBuilder.UseCsp(configurer: cspOptions => SetCspOptions(cspOptions, cspOptionsBuilder, executionEnvironment));
        }

        private static IApplicationBuilder UseExpectCtHeader(this IApplicationBuilder applicationBuilder, ExecutionEnvironment executionEnvironment)
        {
            if (ShouldLogToReportUri(executionEnvironment))
            {
                return applicationBuilder.UseExpectCt(maxAgeInSeconds: 5, enforce: true);
            }

            return applicationBuilder.UseExpectCt(reportUri: @"https://funfair.report-uri.com/r/d/ct/enforce", maxAgeInSeconds: 5, enforce: true);
        }

        private static bool ShouldLogToReportUri(ExecutionEnvironment environment)
        {
            return environment == ExecutionEnvironment.LIVE;
        }
    }
}