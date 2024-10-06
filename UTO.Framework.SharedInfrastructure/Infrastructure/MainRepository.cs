using Dapper.SimpleLoad;
using Dapper.SimpleSave;
using System.Collections.Generic;
using System.Linq;

namespace UTO.Framework.SharedInfrastructure.Infrastructure
{
    public class BaseRepository<T> : ProcRepository
    {
        private readonly DBConnection dbConnection;

        public BaseRepository()
        {
            this.dbConnection = new DBConnection();
        }

        public void AddEntity(T Obj)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                connection.Create(Obj);
                connection.Close();
            }
        }

        public void UpdateEntity(T oldObj, T newObj)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                connection.Update(oldObj, newObj);
                connection.Close();
            }
        }

        public void DeleteEntity(T Obj)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                connection.Delete(Obj);
                connection.Close();
            }
        }

        public T1 GetById<T1>(object param)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                T1 obj = connection.AutoQuery<T1>(param).FirstOrDefault();
                connection.Close();
                return obj;
            }
        }

        public T1 GetById<T1, T2>(object param)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                T1 obj = connection.AutoQuery<T1, T2>(param).FirstOrDefault();
                connection.Close();
                return obj;
            }
        }

        public T1 GetById<T1, T2, T3>(object param)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                T1 obj = connection.AutoQuery<T1, T2, T3>(param).FirstOrDefault();
                connection.Close();
                return obj;
            }
        }

        public T1 GetById<T1, T2, T3, T4>(object param)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                T1 obj = connection.AutoQuery<T1, T2, T3, T4>(param).FirstOrDefault();
                connection.Close();
                return obj;
            }
        }

        public IEnumerable<T> GetAll<T1>()
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var list = connection.AutoQuery<T1>(null).ToList();
                connection.Close();
                return (IEnumerable<T>)list;
            }
        }

        public IEnumerable<T> GetAll<T1, T2>()
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var list = connection.AutoQuery<T1, T2>(null).ToList();
                connection.Close();
                return (IEnumerable<T>)list;
            }
        }

        public IEnumerable<T> GetAll<T1, T2, T3>()
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var list = connection.AutoQuery<T1, T2, T3>(null).ToList();
                connection.Close();
                return (IEnumerable<T>)list;
            }
        }

        public IEnumerable<T> GetAll<T1, T2, T3, T4>()
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var list = connection.AutoQuery<T1, T2, T3, T4>(null).ToList();
                connection.Close();
                return (IEnumerable<T>)list;
            }
        }

        public IEnumerable<T1> SearchForEntity<T1>(object param)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var list = connection.AutoQuery<T1>(param).ToList();
                connection.Close();
                return (IEnumerable<T1>)list;
            }
        }

        public IEnumerable<T1> SearchForEntity<T1, T2>(object param)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var list = connection.AutoQuery<T1, T2>(param).ToList();
                connection.Close();
                return (IEnumerable<T1>)list;
            }
        }

        public IEnumerable<T1> SearchForEntity<T1, T2, T3>(object param)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var list = connection.AutoQuery<T1, T2, T3>(param).ToList();
                connection.Close();
                return (IEnumerable<T1>)list;
            }
        }

        public IEnumerable<T1> SearchForEntity<T1, T2, T3, T4>(object param)
        {
            using (var connection = dbConnection.Connection())
            {
                connection.Open();
                var list = connection.AutoQuery<T1, T2, T3, T4>(param).ToList();
                connection.Close();
                return (IEnumerable<T1>)list;
            }
        }
    }
}
