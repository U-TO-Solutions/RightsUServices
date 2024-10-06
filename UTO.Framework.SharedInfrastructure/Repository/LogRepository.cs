using System.Collections.Generic;
using UTO.Framework.SharedInfrastructure.Data;
using UTO.Framework.SharedInfrastructure.Infrastructure;

namespace UTO.Framework.SharedInfrastructure.Repository
{
    public class LogRepository : BaseRepository<Log>
    {
        public void Add(Log entity)
        {
            base.AddEntity(entity);
        }

        public Log Get(int Id)
        {
            var obj = new { LogId = Id };

            return base.GetById<Log>(obj);
        }

        public IEnumerable<Log> GetAll()
        {
            return base.GetAll<Log>();
        }

        public void Update(Log entity)
        {
            Log oldObj = Get(entity.LogId.Value);
            base.UpdateEntity(oldObj, entity);
        }

        public IEnumerable<Log> SearchFor(object param)
        {
            return base.SearchForEntity<Log>(param);
        }
    }
}
