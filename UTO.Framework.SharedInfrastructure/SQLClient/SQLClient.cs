//using Npgsql;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using UTO.Framework.Shared.Configuration;

namespace UTO.Framework.SharedInfrastructure.MSSQL
{
    public class SQLClient
    {
        private readonly SqlConnection _connection;

        public SQLClient()
        {
            ApplicationConfiguration appConfig = new ApplicationConfiguration();
            string SQLConnectionHost = appConfig.GetConfigurationValue<string>("MSSQLConnectionHost");
            string SQLConnectionDatabase = appConfig.GetConfigurationValue<string>("MSSQLConnectionDatabase");
            string SQLConnectionUsername = appConfig.GetConfigurationValue<string>("MSSQLConnectionUsername");
            string SQLConnectionPassword = appConfig.GetConfigurationValue<string>("MSSQLConnectionPassword");

            _connection = new SqlConnection($"data source={SQLConnectionHost};initial catalog={SQLConnectionDatabase};persist security info=True;user id={SQLConnectionUsername};password={SQLConnectionPassword}");
        }

        public SQLClient(string SQLConnectionHost, string SQLConnectionDatabase, string SQLConnectionUsername, string SQLConnectionPassword)
        {
            _connection = new SqlConnection($"data source={SQLConnectionHost};initial catalog={SQLConnectionDatabase};persist security info=True;user id={SQLConnectionUsername};password={SQLConnectionPassword}");
        }

        public SqlConnection Connection()
        {
            return _connection;
        }
    }
}

