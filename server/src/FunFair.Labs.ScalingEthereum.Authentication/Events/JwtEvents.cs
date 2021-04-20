using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace FunFair.Labs.ScalingEthereum.Authentication.Events
{
    public sealed class JwtEvents : JwtBearerEvents
    {
        private const string BEARER = "Bearer ";
        private readonly ILogger<JwtEvents> _logger;

        public JwtEvents(ILogger<JwtEvents> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private void Error(TokenValidatedContext context, string error)
        {
            this._logger.LogWarning(error);
            context.Fail(error);
        }

        public override Task MessageReceived(MessageReceivedContext context)
        {
            if (context.HttpContext.Request.Headers.ContainsKey(WellKnownHeaders.TokenHeader))
            {
                string token = context.HttpContext.Request.Headers[WellKnownHeaders.TokenHeader];

                if (token.StartsWith(value: BEARER, comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = token.Substring(BEARER.Length)
                                         .Trim();
                }
                else
                {
                    AuthenticateResult.NoResult();
                }
            }
            else
            {
                if (context.Request.Path.StartsWithSegments(other: "/hub/authenticated", comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    if (!context.HttpContext.Request.Query.TryGetValue(key: "access_token", out StringValues token))
                    {
                        AuthenticateResult.NoResult();
                    }
                    else
                    {
                        context.Token = token;
                    }
                }
                else
                {
                    AuthenticateResult.NoResult();
                }
            }

            return Task.CompletedTask;
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                this._logger.LogWarning(exception: context.Exception, message: "JWT Expired");
            }
            else
            {
                this._logger.LogError(exception: context.Exception, message: "Authentication Failed");
            }

            return Task.CompletedTask;
        }

        public override Task TokenValidated(TokenValidatedContext context)
        {
            ClaimsPrincipal? principal = context.Principal;

            if (principal == null)
            {
                this.Error(context: context, error: "Principal not found");

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}