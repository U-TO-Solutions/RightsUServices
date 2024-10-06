using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using UTO.Framework.Shared.Interfaces;

namespace UTO.Framework.Shared.Caching
{
    /// <summary>
    /// TODO: locking pattern with double check
    /// </summary>
    public class InMemoryCache : ICacheStorage
    {
        private static ObjectCache cache { get { return MemoryCache.Default; } }

        void ICacheStorage.Remove<T>(string key)
        {
            cache.Remove(key);
        }

        void ICacheStorage.Store<T>(string key, T data)
        {
            cache.Add(key, data, null);
        }

        void ICacheStorage.Store<T>(string key, T data, TimeSpan slidingExpiration)
        {
            var policy = new CacheItemPolicy
            {
                SlidingExpiration = slidingExpiration
            };

            if (cache.Contains(key))
            {
                cache.Remove(key);
            }

            cache.Add(key, data, policy);
        }

        void ICacheStorage.Store<T>(string key, T data, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = absoluteExpiration,
                SlidingExpiration = slidingExpiration
            };

            if (cache.Contains(key))
            {
                cache.Remove(key);
            }

            cache.Add(key, data, policy);
        }

        T ICacheStorage.Retrieve<T>(string key)
        {
            return cache.Contains(key) ? (T)cache[key] : default(T);
        }

        public bool IsSet(string key)
        {
            return (cache[key] != null);
        }

        public List<string> AllCacheItem()
        {
            List<string> _lstCache = new List<string>();
            foreach (var item in cache)
            {
                _lstCache.Add(item.Key);
            }

            return _lstCache;
        }
    }
}
