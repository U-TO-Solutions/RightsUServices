using System;
using ZeroFormatter;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AcqRightsValidation.Entities
{
    [ZeroFormattable]
    public class LicensePeriod
    {
        [Index(0)]
        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public virtual DateTime LicensePeriodStartFrom { get; set; }
        [Index(1)]
        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public virtual DateTime? LicensePeriodEndTo { get; set; }
        [Index(2)]
        public virtual bool IsPerpetualLicense { get; set; }
    }
}
