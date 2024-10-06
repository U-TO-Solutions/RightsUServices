using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTO.Framework.Shared.Interfaces
{
    public interface ICacheProvider
    {
        void Set<T>(string key, T value);

        void Set<T>(string key, T value, TimeSpan timeout);

        T Get<T>(string key);

        bool Remove(string key);

        bool IsInCache(string key);

        void FlushAllKeys();
    }
}
