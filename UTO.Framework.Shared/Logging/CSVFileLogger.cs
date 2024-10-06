using System;
using System.IO;
using System.Threading.Tasks;
using UTO.Framework.Shared.Enums;
using UTO.Framework.Shared.Guards;
using UTO.Framework.Shared.Helpers;
using UTO.Framework.Shared.Interfaces;
using UTO.Framework.Shared.Models.Logging;

namespace UTO.Framework.Shared.Logging
{
    public class CSVFileLogger : ILogger
    {
        #region class members
        private readonly object filelocker = new object();

        private readonly string _appEnvironment;
        private readonly string _rootLogFolderPath;
        private readonly string _rootAppName;
        private readonly int _appId;
        private readonly string _appName;
        private readonly IConfiguration _configuration;

        private string _folderSeparator = "\\";
        private string _stringExtension = ".csv";

        #endregion

        #region constructors
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        /// <param name="configuration"></param>
        public CSVFileLogger(int appId, string appName, IConfiguration configuration)
        {
            ParameterGuard.AgainstNullParameter<int>(appId);
            ParameterGuard.AgainstNullParameter(appName);
            ParameterGuard.AgainstNullParameter<IConfiguration>(configuration);

            _appId = appId;
            _appName = appName;
            _configuration = configuration;

            _appEnvironment = _configuration.GetConfigurationValue("App.Environment", AppEnvironment.Production.ToString());
            _rootLogFolderPath = _configuration.GetConfigurationValue("RootLogFolderPath");
            _rootAppName = _configuration.GetConfigurationValue("RootAppName", "UTO");
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="configuration"></param>
        public CSVFileLogger(IConfiguration configuration)
        {
            _configuration = configuration;

            _appEnvironment = _configuration.GetConfigurationValue("App.Environment", AppEnvironment.Production.ToString());
            _rootLogFolderPath = _configuration.GetConfigurationValue("RootLogFolderPath");
            _rootAppName = _configuration.GetConfigurationValue("RootAppName", "UTO");

            _appId = _configuration.GetConfigurationValue<int>("AppId", 0);
            _appName = _configuration.GetConfigurationValue("AppName", string.Empty);
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

            LogInternal(level, this._appId, this._appName, message, null);
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

            LogInternal(level, this._appId, this._appName, message, ex);
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        /// <param name="message"></param>
        public void Log(LogLevel level, int appId, string appName, string message)
        {
            ParameterGuard.AgainstNullParameter<LogLevel>(level);
            ParameterGuard.AgainstZeroValue(appId);
            ParameterGuard.AgainstNullStringParameter(appName);
            ParameterGuard.AgainstNullStringParameter(message);

            LogInternal(level, appId, appName, message, null);
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void Log(LogLevel level, int appId, string appName, string message, Exception ex)
        {
            ParameterGuard.AgainstNullParameter<LogLevel>(level);
            ParameterGuard.AgainstZeroValue(appId);
            ParameterGuard.AgainstNullStringParameter(appName);
            ParameterGuard.AgainstNullStringParameter(message);

            ParameterGuard.AgainstNullParameter(ex);

            LogInternal(level, appId, appName, message, ex);
        }

        /// <summary>
        /// Log
        /// </summary>
        /// <param name="logMessage"></param>
        public void Log(LogMessage logMessage)
        {
            ParameterGuard.AgainstNullParameter<LogMessage>(logMessage);

            string appName = string.Empty;

            if (!string.IsNullOrEmpty(logMessage.AppName))
                appName = logMessage.AppName;
            else
                appName = _configuration.GetConfigurationValue("AppName", string.Empty);

            LogInternal(logMessage.LogType, logMessage.AppId, appName, logMessage.Message, logMessage.AppException);
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

            LogInternal(level, this._appId, this._appName, message, null);

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

            LogInternal(level, this._appId, this._appName, message, ex);

            returnValue = true;

            return Task.FromResult(returnValue);
        }

        /// <summary>
        /// LogAsync
        /// </summary>
        /// <param name="level"></param>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<bool> LogAsync(LogLevel level, int appId, string appName, string message)
        {
            bool returnValue = false;

            ParameterGuard.AgainstNullParameter<LogLevel>(level);
            ParameterGuard.AgainstZeroValue(appId);
            ParameterGuard.AgainstNullStringParameter(appName);
            ParameterGuard.AgainstNullStringParameter(message);

            LogInternal(level, appId, appName, message, null);

            returnValue = true;

            return Task.FromResult(returnValue);
        }

        /// <summary>
        /// LogAsync
        /// </summary>
        /// <param name="level"></param>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public Task<bool> LogAsync(LogLevel level, int appId, string appName, string message, Exception ex)
        {
            bool returnValue = false;

            ParameterGuard.AgainstNullParameter<LogLevel>(level);
            ParameterGuard.AgainstZeroValue(appId);
            ParameterGuard.AgainstNullStringParameter(appName);
            ParameterGuard.AgainstNullStringParameter(message);

            ParameterGuard.AgainstNullParameter(ex);

            LogInternal(level, appId, appName, message, ex);

            returnValue = true;

            return Task.FromResult(returnValue);
        }

        Task<bool> ILogger.LogAsync(LogMessage logMessage)
        {
            bool returnValue = false;

            ParameterGuard.AgainstNullParameter<LogMessage>(logMessage);

            string appName = string.Empty;

            if (!string.IsNullOrEmpty(logMessage.AppName))
                appName = logMessage.AppName;
            else
                appName = _configuration.GetConfigurationValue("AppName", string.Empty);

            LogInternal(logMessage.LogType, logMessage.AppId, appName, logMessage.Message, logMessage.AppException);

            returnValue = true;

            return Task.FromResult(returnValue);
        }
        #endregion

        #region private methods
        /// <summary>
        /// Common log method
        /// </summary>
        /// <param name="level"></param>
        /// <param name="appId"></param>
        /// <param name="appName"></param>
        /// <param name="message"></param>
        /// <param name="description"></param>
        /// <param name="ex"></param>
        private void LogInternal(LogLevel level, int appId, string appName, string message, Exception ex)
        {
            ParameterGuard.AgainstZeroValue(appId, "appId");
            ParameterGuard.AgainstNullStringParameter(appName, "appName");
            ParameterGuard.AgainstNullStringParameter(message, "message");

            string appFolderPath = _rootLogFolderPath + _folderSeparator + _rootAppName + _folderSeparator + _appName;

            bool folderExists = Directory.Exists(appFolderPath);

            bool newFileCreated = false;

            if (!folderExists)
            {
                Directory.CreateDirectory(appFolderPath);
            }

            if (ShouldLog(level))
            {
                lock (filelocker)
                {
                    if (!File.Exists(appFolderPath + _folderSeparator + DateTime.Now.ToString("ddMMMyyyy") + _stringExtension))
                        newFileCreated = true;

                    using (StreamWriter streamWriter = new StreamWriter(appFolderPath + _folderSeparator + DateTime.Now.ToString("ddMMMyyyy") + _stringExtension, true))
                    {
                        string headerMessage = string.Empty;
                        var formattedMessage = string.Empty;

                        headerMessage = String.Format("{0},{1},{2},{3},{4},{5}", "Log Level", "App Id", "App Name", "Message", "Stack Trace", "Created Date Time");

                        if (ex != null)
                            formattedMessage = String.Format("{0},{1},{2},{3},{4},{5}", level.ToString(), appId, appName, message, ex.StackTrace, DateTime.Now.ToString("ddMMMyyyy HH:mm:ss"));
                        else
                            formattedMessage = String.Format("{0},{1},{2},{3},{4},{5}", level.ToString(), appId, appName, message, string.Empty, DateTime.Now.ToString("ddMMMyyyy HH:mm:ss"));

                        if (newFileCreated)
                            streamWriter.WriteLine(headerMessage);

                        streamWriter.WriteLine(formattedMessage);

                        streamWriter.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the message should be logged according to Log Level set in config
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool ShouldLog(LogLevel level)
        {
            AppEnvironment appEnvironment = EnumParser.ParseEnum<AppEnvironment>(_appEnvironment);
            LogLevel resolvedLevel;

            if (appEnvironment == AppEnvironment.Production)
                resolvedLevel = LogLevel.Debug;
            else
                resolvedLevel = LogLevel.Trace;

            var configEnumValue = _configuration.GetConfigurationValue("App.LogLevel", LogLevel.Information.ToString());
            if (Enum.TryParse(configEnumValue, true, out resolvedLevel))
            {
                if (level >= resolvedLevel)
                {
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
        #endregion
    }
}
