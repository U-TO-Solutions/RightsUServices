using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace UTO.Framework.SharedInfrastructure.Infrastructure
{
    public class Table1
    {
        public int col1 { get; set; }

        public string col2 { get; set; }

        public double col3 { get; set; }
    }

    public class Table2
    {
        public string time { get; set; }

        public int value { get; set; }
    }

    public class ProcRepository
    {
        private readonly DBConnection dbConnection;

        public ProcRepository()
        {
            this.dbConnection = new DBConnection();
        }

        public void Test()
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var result = connection.QueryMultiple(
                    "TestSP",
                    new { param1 = 1, param2 = 2 },
                    null, null, CommandType.StoredProcedure);

                // read as IEnumerable<dynamic>
                var table1 = result.Read();
                var table2 = result.Read();

                // read as typed IEnumerable
                var table3 = result.Read<Table1>();
                var table4 = result.Read<Table2>();

                ////Assert
                //Assert.IsNotEmpty(table1);
                //Assert.IsNotEmpty(table2);
                //Assert.IsNotEmpty(table3);
                //Assert.IsNotEmpty(table4);
            }
        }

        public IEnumerable<T> ExecuteSQLStmt<T>(string query)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                int queryTimeoutInSeconds = 120;
                var list = SqlMapper.Query<T>(connection, query, null, commandTimeout: queryTimeoutInSeconds, commandType: CommandType.Text);
                connection.Close();
                return (IEnumerable<T>)list.ToList();
            }
        }

        public IEnumerable<T> ExecuteSQLProcedure<T>(string query, DynamicParameters param)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                int queryTimeoutInSeconds = 120;
                var list = SqlMapper.Query<T>(connection, query, param, commandTimeout: queryTimeoutInSeconds, commandType: CommandType.StoredProcedure);
                connection.Close();
                return (IEnumerable<T>)list.ToList();
            }
        }

        public string ExecuteScalar(string query, DynamicParameters param)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                int queryTimeoutInSeconds = 120;
                var result = SqlMapper.ExecuteScalar(connection, query, param, commandTimeout: queryTimeoutInSeconds, commandType: CommandType.StoredProcedure);
                connection.Close();
                return result.ToString();
            }
        }
    }
}
