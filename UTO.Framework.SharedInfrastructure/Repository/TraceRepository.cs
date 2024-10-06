using System.Collections.Generic;
using UTO.Framework.SharedInfrastructure.Data;
using UTO.Framework.SharedInfrastructure.Infrastructure;

namespace UTO.Framework.SharedInfrastructure.Repository
{
    public class TraceRepository : BaseRepository<Trace>
    {
        public void Add(Trace entity)
        {
            base.AddEntity(entity);
        }

        public Trace Get(int Id)
        {
            var obj = new { TraceId = Id };

            return base.GetById<Trace>(obj);
        }

        public IEnumerable<Trace> GetAll()
        {
            return base.GetAll<Trace>();
        }

        public void Update(Trace entity)
        {
            Trace oldObj = Get(entity.TraceId.Value);
            base.UpdateEntity(oldObj, entity);
        }

        public IEnumerable<Trace> SearchFor(object param)
        {
            return base.SearchForEntity<Trace>(param);
        }
    }
}
