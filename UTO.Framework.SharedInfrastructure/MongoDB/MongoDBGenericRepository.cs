using Dapper.SimpleLoad.Impl;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UTO.Framework.SharedInfrastructure.Data;
using UTO.Framework.SharedInfrastructure.Infrastructure;
using UTO.Framework.SharedInfrastructure.MongoDB;

namespace UTO.Framework.SharedInfrastructure.PostgreSQL
{
    public class MongoDBGenericRepository<T> : IRepository<T>
    {
        private readonly MongoDBClient _mongoDBClient;

        public MongoDBGenericRepository()
        {
            _mongoDBClient = new MongoDBClient();
        }

        public MongoDBGenericRepository(string MongoConnection, string MongoDatabase)
        {
            _mongoDBClient = new MongoDBClient(MongoConnection, MongoDatabase);
        }

        public void Add(Collection<T> entity)
        {
            var _collection = _mongoDBClient.Connection().GetCollection<T>(typeof(T).ToString().Split('.').Last());
            _collection.InsertOne(entity.CollectionStore);
        }

        public void Delete(int Id)
        {
            try
            {
                var _collection = _mongoDBClient.Connection().GetCollection<T>(typeof(T).ToString().Split('.').Last());
                var filterId = Builders<T>.Filter.Eq("TitleCode", Id);
                _collection.FindOneAndDelete(filterId);
            }
            catch (Exception)
            {
                //Do not remove this. if object is already deleted then this exception occurs.
            }
        }

        public void DeleteAll()
        {
            throw new NotImplementedException();
        }

        public T Get(int Id)
        {
            try
            {
                var _collection = _mongoDBClient.Connection().GetCollection<T>(typeof(T).ToString().Split('.').Last());
                var filterId = Builders<T>.Filter.Eq("TitleCode", Id);
                return _collection.Find(filterId).FirstOrDefault();
            }
            catch (Exception)
            {
                //Do not remove this. if object is not found then this exception occurs.
            }
            return default(T);
        }

        public IEnumerable<Collection<T>> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Collection<T>> SearchFor(object param)
        {
            throw new NotImplementedException();
        }

        public void Update(Collection<T> entity)
        {
            throw new NotImplementedException();
        }
    }
}
