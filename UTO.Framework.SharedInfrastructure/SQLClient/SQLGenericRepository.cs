using Newtonsoft.Json;
//using Npgsql;
//using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using UTO.Framework.SharedInfrastructure.Data;
using UTO.Framework.SharedInfrastructure.Infrastructure;

namespace UTO.Framework.SharedInfrastructure.MSSQL
{
    public class SQLGenericRepository<T> : IRepository<T>
    {
        private readonly SQLClient _SQLClient;

        public SQLGenericRepository()
        {
            _SQLClient = new SQLClient();
        }

        public SQLGenericRepository(string SQLConnectionHost, string SQLConnectionDatabase, string SQLConnectionUsername, string SQLConnectionPassword)
        {
            _SQLClient = new SQLClient(SQLConnectionHost, SQLConnectionDatabase, SQLConnectionUsername, SQLConnectionPassword);
        }

        private void openConnection()
        {
            _SQLClient.Connection().Open();
        }

        private void closeConnection()
        {
            _SQLClient.Connection().Close();
        }

        public void Add(Collection<T> entity)
        {
            openConnection();

            using (var command = new SqlCommand($"INSERT INTO \"{typeof(T).ToString().Split('.').Last()}\"(\"TitleCode\",\"CollectionStore\") VALUES(@titleCode, @collectionStore)", _SQLClient.Connection()))
            {
                command.Parameters.Add(new SqlParameter("titleCode", System.Data.SqlDbType.BigInt) { Value = entity.TitleCode });
                command.Parameters.Add(new SqlParameter("collectionStore", System.Data.SqlDbType.NVarChar) { Value = JsonConvert.SerializeObject(entity.CollectionStore) });
                command.ExecuteNonQuery();
            }

            closeConnection();
        }

        public void Delete(int Id)
        {
            _SQLClient.Connection().Open();

            using (var command = new SqlCommand($"Delete From \"{typeof(T).ToString().Split('.').Last()}\" Where \"TitleCode\"={Id};", _SQLClient.Connection()))
            {
                command.ExecuteNonQuery();
            }

            closeConnection();
        }

        public void DeleteAll()
        {
            _SQLClient.Connection().Open();

            using (var command = new SqlCommand($"Truncate Table \"{typeof(T).ToString().Split('.').Last()}\";", _SQLClient.Connection()))
            {
                command.CommandTimeout = 600;
                command.ExecuteNonQuery();
            }

            closeConnection();
        }

        public void ClearMemory()
        {
            _SQLClient.Connection().Open();

            using (var command = new SqlCommand($"VACUUM FULL VERBOSE;", _SQLClient.Connection()))
            {
                command.ExecuteNonQuery();
            }

            closeConnection();
        }

        public T Get(int Id)
        {
            openConnection();

            string query = $"SELECT \"CollectionStore\" from \"{typeof(T).ToString().Split('.').Last()}\" where \"TitleCode\"={Id};";
            SqlCommand command = new SqlCommand(query, _SQLClient.Connection());
            SqlDataReader dr = command.ExecuteReader();
            string result = string.Empty;
            while (dr.Read())
            {
                result = dr[0].ToString();
            }

            closeConnection();

            return JsonConvert.DeserializeObject<T>(result);
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
