using Dapper.SimpleSave;
using System;

namespace UTO.Framework.SharedInfrastructure.Data
{
    [Table("Trace")]
    public partial class Trace
    {
        [PrimaryKey]
        public int? TraceId { get; set; }
        public int UserId { get; set; }
        public string RequestId { get; set; }
        public bool IsSuccess { get; set; }
        public int RequestLength { get; set; }
        public int ResponseLength { get; set; }
        public int HttpStatusCode { get; set; }
        public string HttpStatusDescription { get; set; }
        public string AuthenticationKey { get; set; }
        public string UserAgent { get; set; }
        public string ServerName { get; set; }
        public string ClientIpAddress { get; set; }
        public string RequestUri { get; set; }
        public string RequestMethod { get; set; }
        public string Method { get; set; }
        public int TimeTaken { get; set; }
        public Nullable<System.DateTime> RequestDateTime { get; set; }
        public Nullable<System.DateTime> ResponseDateTime { get; set; }
        public string RequestContent { get; set; }
        public string ResponseContent { get; set; }

        public string ToStringContent()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
