using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace FunFair.Labs.ScalingEthereum.Server.Middleware
{
    /// <summary>
    ///     Middleware to set wss://self in CSP to current request host and port (connect-src requires explicit host and port which is not known until runtime)
    /// </summary>
    public sealed class CorsHttpsHostnameMiddleware
    {
        private const string CORS_HEADER = "Access-Control-Allow-Origin";
        private readonly RequestDelegate _next;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="next">The next delegate to execute</param>
        public CorsHttpsHostnameMiddleware(RequestDelegate next)
        {
            this._next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        ///     Invoke the middleware
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task InvokeAsync(HttpContext context)
        {
            if (context.Response.Headers.TryGetValue(key: CORS_HEADER, out StringValues content))
            {
                string headerContent = content;
                headerContent = headerContent.Replace(oldValue: "https://host", $"https://{context.Request.Host}");
                context.Response.Headers[CORS_HEADER] = headerContent;
            }

            return this._next(context);
        }
    }
}