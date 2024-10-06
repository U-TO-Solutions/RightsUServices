using System;
using System.Collections.Generic;

namespace UTO.Framework.Shared.Interfaces
{
    public interface ICacheStorage
    {
        void Remove<T>(string key);
        void Store<T>(string key, T data);
        void Store<T>(string key, T data, TimeSpan slidingExpiration);
        void Store<T>(string key, T data, DateTime absoluteExpiration, TimeSpan slidingExpiration);
        T Retrieve<T>(string key);
        bool IsSet(string key);
        List<string> AllCacheItem();
    }
}
