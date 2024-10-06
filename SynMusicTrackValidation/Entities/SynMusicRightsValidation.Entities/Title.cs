using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace SynMusicRightsValidation.Entities
{
    [ZeroFormattable]
    public class Title
    {
        [Index(0)]
        public virtual int ID { get; set; }
        [Index(1)]
        public virtual int TitleCode { get; set; }
        [Index(2)]
        public virtual int RightsCode { get; set; }
        [Index(3)]
        public virtual string Validateion_Status { get; set; }
    }
}
