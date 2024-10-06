using System.Collections.Generic;
using UTO.Framework.SharedInfrastructure.Data;

namespace UTO.Framework.SharedInfrastructure.Infrastructure
{
    public interface IRepository<T>
    {
        T Get(int Id);
        IEnumerable<Collection<T>> GetAll();
        void Add(Collection<T> entity);
        void Delete(int Id);
        void DeleteAll();
        void Update(Collection<T> entity);
        IEnumerable<Collection<T>> SearchFor(object param);
    }
}
