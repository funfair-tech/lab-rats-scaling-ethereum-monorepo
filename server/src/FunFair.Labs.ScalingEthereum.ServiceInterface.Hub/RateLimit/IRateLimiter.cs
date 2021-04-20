using Microsoft.AspNetCore.SignalR;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.RateLimit
{
    /// <summary>
    ///     Rate Limiter, check if the caller to a hub should be rate limited
    /// </summary>
    public interface IRateLimiter
    {
        /// <summary>
        ///     Checks if the caller to a hub should be rate limited
        /// </summary>
        /// <param name="context">The HubCallerContext</param>
        /// <returns>True if the caller should be rate limited</returns>
        bool ShouldLimit(HubCallerContext context);
    }
}