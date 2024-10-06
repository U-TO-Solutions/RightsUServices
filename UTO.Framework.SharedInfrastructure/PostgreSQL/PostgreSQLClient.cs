using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using UTO.Framework.Shared.Configuration;

namespace UTO.Framework.SharedInfrastructure.PostgreSQL
{
    public class PostgreSQLClient
    {
        private readonly NpgsqlConnection _connection;

        public PostgreSQLClient()
        {
            ApplicationConfiguration appConfig = new ApplicationConfiguration();
            string postgreSQLConnectionHost = appConfig.GetConfigurationValue<string>("PostgreSQLConnectionHost");
            string postgreSQLConnectionDatabase = appConfig.GetConfigurationValue<string>("PostgreSQLConnectionDatabase");
            string postgreSQLConnectionUsername = appConfig.GetConfigurationValue<string>("PostgreSQLConnectionUsername");
            string postgreSQLConnectionPassword = appConfig.GetConfigurationValue<string>("PostgreSQLConnectionPassword");

            _connection = new NpgsqlConnection($"Host={postgreSQLConnectionHost};Username={postgreSQLConnectionUsername};Password={postgreSQLConnectionPassword};Database={postgreSQLConnectionDatabase}");
        }

        public PostgreSQLClient(string PostgreSQLConnectionHost, string PostgreSQLConnectionDatabase, string PostgreSQLConnectionUsername, string PostgreSQLConnectionPassword)
        {
            _connection = new NpgsqlConnection($"Host={PostgreSQLConnectionHost};Username={PostgreSQLConnectionUsername};Password={PostgreSQLConnectionPassword};Database={PostgreSQLConnectionDatabase}");
        }

        public NpgsqlConnection Connection()
        {
            return _connection;
        }
    }
}
