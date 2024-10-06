using Dapper.SimpleSave;
using System;

namespace UTO.Framework.SharedInfrastructure.Data
{
    [Table("Log")]
    public partial class Log
    {
        [PrimaryKey]
        public int? LogId { get; set; }
        public string RequestId { get; set; }
        public string LogLevel { get; set; }
        public int AppId { get; set; }
        public string AppName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public string ToStringContent()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
