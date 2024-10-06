using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using UTO.Framework.Shared.Configuration;
using UTO.Framework.Shared.Interfaces;

namespace UTO.Framework.SharedInfrastructure.Caching
{
    public class RedisCache : ICacheProvider
    {
        ConnectionMultiplexer redis;
        private string redisServer;
        private string redisServerPort;
        private int _database = 0;

        public RedisCache()
        {
            ApplicationConfiguration config = new ApplicationConfiguration();

            redisServer = config.GetConfigurationValue("RedisServer");
            _database = config.GetConfigurationValue<int>("RedisServerDB");
            redisServerPort = config.GetConfigurationValue("RedisServerPort");

            redis = ConnectionMultiplexer.Connect(redisServer + ":" + redisServerPort + ",allowAdmin=true");
        }

        public RedisCache(int database)
        {
            ApplicationConfiguration config = new ApplicationConfiguration();

            redisServer = config.GetConfigurationValue("RedisServer");
            redisServerPort = config.GetConfigurationValue("RedisServerPort");

            redis = ConnectionMultiplexer.Connect(redisServer + ":" + redisServerPort + ",allowAdmin=true");
            _database = database;
        }

        public RedisCache(string RedisServer, string RedisServerPort)
        {
            ApplicationConfiguration config = new ApplicationConfiguration();

            redisServer = config.GetConfigurationValue(RedisServer);
            _database = config.GetConfigurationValue<int>("RedisServerDB");
            redisServerPort = config.GetConfigurationValue(RedisServerPort);

            redis = ConnectionMultiplexer.Connect(redisServer + ":" + redisServerPort + ",allowAdmin=true");
        }

        public RedisCache(string RedisServer, string RedisServerPort, string RedisServerDB)
        {
            ApplicationConfiguration config = new ApplicationConfiguration();

            redisServer = config.GetConfigurationValue(RedisServer);
            _database = config.GetConfigurationValue<int>(RedisServerDB);
            redisServerPort = config.GetConfigurationValue(RedisServerPort);

            redis = ConnectionMultiplexer.Connect(redisServer + ":" + redisServerPort + ",allowAdmin=true");
        }

        public void Set<T>(string key, T value)
        {
            this.Set(key, value, TimeSpan.FromDays(365));
        }

        public void Set<T>(string key, T value, TimeSpan timeout)
        {
            IDatabase db = redis.GetDatabase(_database);
            db.StringSet(key, JsonConvert.SerializeObject(value), timeout);
        }

        public T Get<T>(string key)
        {
            try
            {
                IDatabase db = redis.GetDatabase(_database);
                var res = db.StringGet(key);

                if (res.IsNull)
                    return default(T);
                else
                    return JsonConvert.DeserializeObject<T>(res);
            }
            catch
            {
                return default(T);
            }
        }

        public bool Remove(string key)
        {
            IDatabase db = redis.GetDatabase(_database);
            return db.KeyDelete(key);
        }

        public bool IsInCache(string key)
        {
            IDatabase db = redis.GetDatabase(_database);
            return db.KeyExists(key);
        }

        public void FlushAllKeys()
        {
            var server = redis.GetServer(redisServer + ":" + redisServerPort);
            server.FlushDatabase(_database);
        }
    }
}
