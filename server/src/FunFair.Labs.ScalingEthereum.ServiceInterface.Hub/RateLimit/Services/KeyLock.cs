using System;
using System.Collections.Generic;
using NonBlocking;

namespace FunFair.Labs.ScalingEthereum.ServiceInterface.Hub.RateLimit.Services
{
    /// <summary>
    ///     Class to take locks on keys
    /// </summary>
    /// <typeparam name="T">The Type for the key</typeparam>
    public sealed class KeyLock<T>
        where T : notnull
    {
        private readonly ConcurrentDictionary<T, object> _locks;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="comparer">The comparer to use to identify keys</param>
        public KeyLock(IEqualityComparer<T> comparer)
        {
            this._locks = new ConcurrentDictionary<T, object>(comparer);
        }

        /// <summary>
        ///     Get an object to lock on for the key
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The object to lock on</returns>
        public object GetLock(T key)
        {
            return this.GetLockCommon(key);
        }

        private object GetLockCommon(T key)
        {
            return this._locks.GetOrAdd(key: key, valueFactory: _ => new object());
        }

        /// <summary>
        ///     Run under lock and return TResult
        /// </summary>
        /// <typeparam name="TResult">The type to return</typeparam>
        /// <param name="key">The key to use</param>
        /// <param name="func">The func to execute</param>
        /// <returns>The result of the func</returns>
        public TResult RunWithLock<TResult>(T key, Func<TResult> func)
        {
            lock (this.GetLockCommon(key))
            {
                return func();
            }
        }

        /// <summary>
        ///     Run action under lock
        /// </summary>
        /// <param name="key">The key to use</param>
        /// <param name="action">The action to run</param>
        public void RunWithLock(T key, Action action)
        {
            lock (this.GetLockCommon(key))
            {
                action();
            }
        }

        /// <summary>
        ///     Remove a lock, or do nothing if no key exists
        /// </summary>
        /// <param name="key">The key</param>
        public void RemoveLock(T key)
        {
            this._locks.TryRemove(key: key, value: out _);
        }
    }
}