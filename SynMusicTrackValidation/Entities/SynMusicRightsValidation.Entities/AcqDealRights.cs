using System.Collections.Generic;
using ZeroFormatter;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SynMusicRightsValidation.Entities
{
        [ZeroFormattable]
        public class AcqDealRights
    {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            [Index(0)]
            public virtual string _id { get; set; }
            [Index(1)]
            public virtual int TitleCode { get; set; }
            //[Index(2)]
            //public virtual List<Right> RightList { get; set; }
        }
    }
