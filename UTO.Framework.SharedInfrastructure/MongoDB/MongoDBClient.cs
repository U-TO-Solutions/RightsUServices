using MongoDB.Driver;
using UTO.Framework.Shared.Configuration;

namespace UTO.Framework.SharedInfrastructure.MongoDB
{
    public class MongoDBClient
    {
        private readonly IMongoDatabase _database;
        public IMongoClient _client;

        public MongoDBClient()
        {
            ApplicationConfiguration appConfig = new ApplicationConfiguration();
            string monogoConnection = appConfig.GetConfigurationValue<string>("MongoConnection");
            string monogoDB = appConfig.GetConfigurationValue<string>("MongoDatabase");

            _client = new MongoClient(monogoConnection);
            _database = _client.GetDatabase(monogoDB);
        }

        public MongoDBClient(string MongoConnection, string MongoDatabase)
        {
            ApplicationConfiguration appConfig = new ApplicationConfiguration();
            string monogoConnection = appConfig.GetConfigurationValue<string>(MongoConnection);
            string monogoDB = appConfig.GetConfigurationValue<string>(MongoDatabase);

            _client = new MongoClient(monogoConnection);
            _database = _client.GetDatabase(monogoDB);
        }

        public IMongoDatabase Connection()
        {
            return _database;
        }
    }
}
