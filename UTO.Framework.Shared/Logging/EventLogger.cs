using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UTO.Framework.Shared.Enums;
using UTO.Framework.Shared.Guards;
using UTO.Framework.Shared.Helpers;
using UTO.Framework.Shared.Interfaces;
using UTO.Framework.Shared.Models.Logging;

namespace UTO.Framework.Shared.Logging
{
    public class EventLogger : ILogger
    {
        #region class members
        private readonly string _eventLogName;
        private readonly int _eventId;
        private readonly string _eventSource;
        private readonly IConfiguration _configuration;
        private readonly EventLog eventLog = null;
        #endregion

        #region constructors
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="logName"></param>
        /// <param name="configuration"></param>
        public EventLogger(string logName, IConfiguration configuration)
        {
            ParameterGuard.AgainstNullStringParameter(logName);
            ParameterGuard.AgainstNullParameter(configuration);

            _eventLogName = logName;
            _configuration = configuration;

            if (EventLog.Exists(_eventLogName))
                eventLog = new EventLog(_eventLogName);
            else
                eventLog = new EventLog("Application");
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="logName"></param>
        /// <param name="configuration"></param>
        public EventLogger(string logName, int eventId, string source, IConfiguration configuration)
        {
            ParameterGuard.AgainstNullStringParameter(logName);
            ParameterGuard.AgainstNullParameter<int>(eventId);
            ParameterGuard.AgainstNullStringParameter(source);
            ParameterGuard.AgainstNullParameter(configuration);

            _eventLogName = logName;
            _eventId = eventId;
            _eventSource = source;
            _configuration = configuration;

            if (EventLog.Exists(_eventLogName))
                eventLog = new EventLog(_eventLogName);
            else
                eventLog = new EventLog("Application");
        }
        #endregion

        #region public methods

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LogLevel level, string message)
        {
            ParameterGuard.AgainstNullParameter<LogLevel>(level);
            ParameterGuard.AgainstNullStringParameter(message);

            LogInternal(level, this._eventId, this._eventSource, message, null);
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Log(LogLevel level, string message, Exception ex)
        {
            ParameterGuard.AgainstNullParameter<LogLevel>(level);
            ParameterGuard.AgainstNullStringParameter(message);
            ParameterGuard.AgainstNullParameter(ex);

            LogInternal(level, this._eventId, this._eventSource, message, ex);
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="eventId"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public void Log(LogLevel level, int eventId, string source, string message)
        {
            ParameterGuard.AgainstNullParameter<LogLevel>(level);
            ParameterGuard.AgainstZeroValue(eventId);
            ParameterGuard.AgainstNullStringParameter(source);
            ParameterGuard.AgainstNullStringParameter(message);

            LogInternal(level, eventId, source, message, null);
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="eventId"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Log(LogLevel level, int eventId, string source, string message, Exception ex)
        {
            ParameterGuard.AgainstNullParameter<LogLevel>(level);
            ParameterGuard.AgainstZeroValue(eventId);
            ParameterGuard.AgainstNullStringParameter(source);
            ParameterGuard.AgainstNullStringParameter(message);

            ParameterGuard.AgainstNullParameter(ex);

            LogInternal(level, eventId, source, message, ex);
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="logMessage"></param>
        public void Log(LogMessage logMessage)
        {
            ParameterGuard.AgainstNullParameter<LogMessage>(logMessage);

            string eventSource = string.Empty;

            eventSource = _configuration.GetConfigurationValue("AppName", string.Empty);

            if (!string.IsNullOrEmpty(eventSource))
                eventSource = logMessage.AppName;

            if (EventLog.SourceExists(eventSource))
            {
                string formattedLogMessage = JsonFormatter.Format(JsonConvert.SerializeObject(logMessage));
                eventLog.Source = eventSource;
                //set application name as source
                LogInternal(LogLevel.Error, logMessage.AppId, eventSource, logMessage.Message + "\r\n" + formattedLogMessage, logMessage.AppException);
            }
        }

        /// <summary>
        /// LogAsync
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<bool> LogAsync(LogLevel level, string message)
        {
            bool returnValue = false;

            ParameterGuard.AgainstNullParameter<LogLevel>(level);
            ParameterGuard.AgainstNullStringParameter(message);

            LogInternal(level, this._eventId, this._eventSource, message, null);

            returnValue = true;

            return Task.FromResult(returnValue);
        }

        /// <summary>
        /// LogAsync
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public Task<bool> LogAsync(LogLevel level, string message, Exception ex)
        {
            bool returnValue = false;

            ParameterGuard.AgainstNullParameter<LogLevel>(level);
            ParameterGuard.AgainstNullStringParameter(message);
            ParameterGuard.AgainstNullParameter(ex);

            LogInternal(level, this._eventId, this._eventSource, message, ex);

            returnValue = true;

            return Task.FromResult(returnValue);
        }

        /// <summary>
        /// LogAsync
        /// </summary>
        /// <param name="level"></param>
        /// <param name="eventId"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<bool> LogAsync(LogLevel level, int eventId, string source, string message)
        {
            bool returnValue = false;

            ParameterGuard.AgainstNullParameter<LogLevel>(level);
            ParameterGuard.AgainstZeroValue(eventId);
            ParameterGuard.AgainstNullStringParameter(source);
            ParameterGuard.AgainstNullStringParameter(message);

            LogInternal(level, eventId, source, message, null);

            returnValue = true;

            return Task.FromResult(returnValue);
        }

        /// <summary>
        /// LogAsync
        /// </summary>
        /// <param name="level"></param>
        /// <param name="eventId"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public Task<bool> LogAsync(LogLevel level, int eventId, string source, string message, Exception ex)
        {
            bool returnValue = false;

            ParameterGuard.AgainstNullParameter<LogLevel>(level);
            ParameterGuard.AgainstZeroValue(eventId);
            ParameterGuard.AgainstNullStringParameter(source);
            ParameterGuard.AgainstNullStringParameter(message);

            ParameterGuard.AgainstNullParameter(ex);

            LogInternal(level, eventId, source, message, ex);

            returnValue = true;

            return Task.FromResult(returnValue);
        }

        Task<bool> ILogger.LogAsync(LogMessage logMessage)
        {
            bool returnValue = false;

            ParameterGuard.AgainstNullParameter<LogMessage>(logMessage);

            string eventSource = string.Empty;

            eventSource = _configuration.GetConfigurationValue("AppName", string.Empty);

            if (EventLog.SourceExists(eventSource))
            {
                string formattedLogMessage = JsonFormatter.Format(JsonConvert.SerializeObject(logMessage));
                eventLog.Source = eventSource;
                //set application name as source

                LogInternal(logMessage.LogType, logMessage.AppId, eventSource, logMessage.Message + "\r\n" + formattedLogMessage, logMessage.AppException);
            }

            returnValue = true;

            return Task.FromResult(returnValue);
        }
        #endregion

        #region private methods
        /// <summary>
        /// Common log method
        /// </summary>
        /// <param name="level"></param>
        /// <param name="eventId"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="description"></param>
        /// <param name="ex"></param>
        private void LogInternal(LogLevel level, int eventId, string source, string message, Exception ex)
        {
            ParameterGuard.AgainstZeroValue(eventId);
            ParameterGuard.AgainstNullStringParameter(source);
            ParameterGuard.AgainstNullStringParameter(message);

            if (ShouldLog(level, out EventLogEntryType eventLogEntryType))
            {
                if (EventLog.SourceExists(source))
                {
                    //set application name as source
                    eventLog.Source = source;
                    eventLog.WriteEntry(message, eventLogEntryType, eventId);
                }
            }
        }

        /// <summary>
        /// Checks if the message should be logged according to Log Level set in config
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool ShouldLog(LogLevel level, out EventLogEntryType eventLogEntryType)
        {
            eventLogEntryType = EventLogEntryType.Information;
            LogLevel resolvedLevel = LogLevel.Information;
            var configEnumValue = _configuration.GetConfigurationValue("App.LogLevel", LogLevel.Information.ToString());
            if (Enum.TryParse(configEnumValue, true, out resolvedLevel))
            {
                if (level >= resolvedLevel)
                {
                    eventLogEntryType = GetEventLogEntryType(level);
                    return true;
                }
                else
                    return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Get EventLogEntryType From LogLevel
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        private EventLogEntryType GetEventLogEntryType(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Information)
                return EventLogEntryType.Information;
            if (logLevel == LogLevel.Warning)
                return EventLogEntryType.Warning;
            if (logLevel == LogLevel.Error || logLevel == LogLevel.Critical)
                return EventLogEntryType.Error;
            return EventLogEntryType.Information;
        }
        #endregion
    }
}
