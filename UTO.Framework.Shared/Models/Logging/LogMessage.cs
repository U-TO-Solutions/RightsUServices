using System;
using UTO.Framework.Shared.Enums;

namespace UTO.Framework.Shared.Models.Logging
{
    public class LogMessage
    {
        public string RequestId { get; set; }
        public LogLevel LogType { get; set; }
        public int AppId { get; set; }
        public string AppName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public Exception AppException { get; set; }
    }
}
