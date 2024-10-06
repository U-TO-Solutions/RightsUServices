using System;
using System.Threading.Tasks;
using UTO.Framework.Shared.Enums;
using UTO.Framework.Shared.Interfaces;
using UTO.Framework.Shared.Models.Logging;
using UTO.Framework.SharedInfrastructure.Data;
using UTO.Framework.SharedInfrastructure.Repository;

namespace UTO.Framework.SharedInfrastructure.Logging
{
    public class DBLogger : ILogger
    {
        private readonly LogRepository logRepository = new LogRepository();

        public void Log(LogMessage logMessage)
        {
            Log log = new Log();
            log.LogLevel = logMessage.LogType.ToString();
            log.RequestId = logMessage.RequestId;
            log.AppId = logMessage.AppId;
            log.AppName = logMessage.AppName;
            log.Message = logMessage.Message;
            log.StackTrace = logMessage.StackTrace;
            log.CreatedDateTime = logMessage.CreatedDateTime;
            logRepository.AddEntity(log);
        }

        public void Log(LogLevel level, string message)
        {
            throw new NotImplementedException();
        }

        public void Log(LogLevel level, string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void Log(LogLevel level, int eventId, string source, string message)
        {
            throw new NotImplementedException();
        }

        public void Log(LogLevel level, int eventId, string source, string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogAsync(LogLevel level, string message)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogAsync(LogLevel level, string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogAsync(LogLevel level, int eventId, string source, string message)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogAsync(LogLevel level, int eventId, string source, string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogAsync(LogMessage logMessage)
        {
            throw new NotImplementedException();
        }
    }
}
