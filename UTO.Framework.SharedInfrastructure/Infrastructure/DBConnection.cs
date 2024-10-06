using System.Data.SqlClient;
using UTO.Framework.Shared.Configuration;

namespace UTO.Framework.SharedInfrastructure.Infrastructure
{
    public class DBConnection
    {
        private static string ConnectionString;

        public DBConnection()
        {
            ApplicationConfiguration app = new ApplicationConfiguration();
            ConnectionString = app.GetConnectionString("DBConnection");
        }

        public DBConnection(string DBConnection)
        {
            ApplicationConfiguration app = new ApplicationConfiguration();
            ConnectionString = app.GetConnectionString(DBConnection);
        }

        public SqlConnection Connection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
