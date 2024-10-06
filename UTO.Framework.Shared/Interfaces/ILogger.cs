using System;
using System.Threading.Tasks;
using UTO.Framework.Shared.Enums;
using UTO.Framework.Shared.Models.Logging;

namespace UTO.Framework.Shared.Interfaces
{
    public interface ILogger
    {
        Task<bool> LogAsync(LogLevel level, string message);
        Task<bool> LogAsync(LogLevel level, string message, Exception ex);

        Task<bool> LogAsync(LogLevel level, int appId, string appName, string message);
        Task<bool> LogAsync(LogLevel level, int appId, string appName, string message, Exception ex);

        Task<bool> LogAsync(LogMessage logMessage);

        void Log(LogLevel level, string message);
        void Log(LogLevel level, string message, Exception ex);

        void Log(LogLevel level, int appId, string appName, string message);
        void Log(LogLevel level, int appId, string appName, string message, Exception ex);

        void Log(LogMessage logMessage);
    }
}
