using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTO.Framework.SharedInfrastructure.Data
{
    public class Collection<T>
    {
        public int TitleCode { get; set; }

        public T CollectionStore { get; set; }
    }
}
