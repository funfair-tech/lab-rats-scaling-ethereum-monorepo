using System;
using FunFair.Common.Extensions;
using FunFair.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.RateLimit.Services
{
    public sealed class RateLimiter : IRateLimiter
    {
        // the number of seconds to use for each limit window
        private const int LIMIT_WINDOW = 1;

        // the number of requests that should be allowed in the window
        private const int RATE_LIMIT = 2;
        private readonly IDateTimeSource _dateTimeSource;
        private readonly KeyLock<string> _keyLock;

        private readonly IMemoryCache _memoryCache;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="dateTimeSource">The IDateTimeSource</param>
        public RateLimiter(IDateTimeSource dateTimeSource)
        {
            this._memoryCache = new MemoryCache(new MemoryCacheOptions());
            this._dateTimeSource = dateTimeSource ?? throw new ArgumentNullException(nameof(dateTimeSource));
            this._keyLock = new KeyLock<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public bool ShouldLimit(HubCallerContext context)
        {
            try
            {
                return this._keyLock.RunWithLock(key: context.ConnectionId,
                                                 func: () =>
                                                       {
                                                           DateTime roundedDateTime = this.GetRoundedDateTime();

                                                           string cacheKey = GetCacheKey(context: context, roundedDateTime: roundedDateTime);

                                                           if (this._memoryCache.TryGetValue(key: cacheKey, out int counter))
                                                           {
                                                               if (counter + 1 > RATE_LIMIT)
                                                               {
                                                                   // hit the rate limit for this period already, just return true
                                                                   return true;
                                                               }
                                                           }

                                                           this._memoryCache.Set(key: cacheKey, counter + 1, TimeSpan.FromSeconds(LIMIT_WINDOW * 2));

                                                           return false;
                                                       });
            }
            finally
            {
                this._keyLock.RemoveLock(context.ConnectionId);
            }
        }

        /// <summary>
        ///     Return the datetime rounded down to the nearest LIMIT_WINDOW
        /// </summary>
        /// <returns>The rounded datetime</returns>
        private DateTime GetRoundedDateTime()
        {
            return this._dateTimeSource.UtcNow()
                       .Round();
        }

        /// <summary>
        ///     Get the cache key for the HubCallerContext and time
        /// </summary>
        /// <param name="context">The HubCallerContext</param>
        /// <param name="roundedDateTime">The time, rounded down to the closest LIMIT_WINDOW</param>
        /// <returns>The cache key</returns>
        private static string GetCacheKey(HubCallerContext context, DateTime roundedDateTime)
        {
            return $"RATE_LIMIT:{context.ConnectionId}:{roundedDateTime.ToUniversalTime()}";
        }
    }
}