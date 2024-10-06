using Newtonsoft.Json;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UTO.Framework.SharedInfrastructure.Data;
using UTO.Framework.SharedInfrastructure.Infrastructure;

namespace UTO.Framework.SharedInfrastructure.PostgreSQL
{
    public class PostgreSQLGenericRepository<T> : IRepository<T>
    {
        private readonly PostgreSQLClient _postgreSQLClient;

        public PostgreSQLGenericRepository()
        {
            _postgreSQLClient = new PostgreSQLClient();
        }

        public PostgreSQLGenericRepository(string PostgreSQLConnectionHost, string PostgreSQLConnectionDatabase, string PostgreSQLConnectionUsername, string PostgreSQLConnectionPassword)
        {
            _postgreSQLClient = new PostgreSQLClient(PostgreSQLConnectionHost, PostgreSQLConnectionDatabase, PostgreSQLConnectionUsername, PostgreSQLConnectionPassword);
        }

        private void openConnection()
        {
            _postgreSQLClient.Connection().Open();
        }

        private void closeConnection()
        {
            _postgreSQLClient.Connection().Close();
        }

        public void Add(Collection<T> entity)
        {
            openConnection();

            using (var command = new NpgsqlCommand($"INSERT INTO public.\"{typeof(T).ToString().Split('.').Last()}\"(\"TitleCode\",\"CollectionStore\") VALUES(@titleCode, @collectionStore)", _postgreSQLClient.Connection()))
            {
                command.Parameters.Add(new NpgsqlParameter("titleCode", NpgsqlDbType.Integer) { Value = entity.TitleCode });
                command.Parameters.Add(new NpgsqlParameter("collectionStore", NpgsqlDbType.Jsonb) { Value = JsonConvert.SerializeObject(entity.CollectionStore) });
                command.ExecuteNonQuery();
            }

            closeConnection();
        }

        public void Delete(int Id)
        {
            _postgreSQLClient.Connection().Open();

            using (var command = new NpgsqlCommand($"Delete From public.\"{typeof(T).ToString().Split('.').Last()}\" Where \"TitleCode\"={Id};", _postgreSQLClient.Connection()))
            {
                command.ExecuteNonQuery();
            }

            closeConnection();
        }

        public void DeleteAll()
        {
            _postgreSQLClient.Connection().Open();

            using (var command = new NpgsqlCommand($"Delete From public.\"{typeof(T).ToString().Split('.').Last()}\";", _postgreSQLClient.Connection()))
            {
                command.ExecuteNonQuery();
            }

            closeConnection();
        }

        public T Get(int Id)
        {
            openConnection();

            string query = $"SELECT \"CollectionStore\" from public.\"{typeof(T).ToString().Split('.').Last()}\" where \"TitleCode\"={Id};";
            NpgsqlCommand command = new NpgsqlCommand(query, _postgreSQLClient.Connection());
            NpgsqlDataReader dr = command.ExecuteReader();
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
