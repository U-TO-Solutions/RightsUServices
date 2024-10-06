using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UTO.Framework.Shared.Configuration;
using UTO.Framework.Shared.Enums;
using UTO.Framework.Shared.Extensions;
using UTO.Framework.Shared.Factories;
using UTO.Framework.Shared.Interfaces;
using UTO.Framework.Shared.Logging;
using UTO.Framework.Shared.Models.Domain;
using UTO.Framework.Shared.Models.Logging;

namespace UTO.Framework.SharedInfrastructure.Client
{
    /// <summary>
    /// Base HttpClient
    /// </summary>
    public class BaseHttpClient : IHttpClient, IDisposable
    {
        #region Global variables
        IConfiguration _configuration;
        ILogger _eventLogger;
        Uri _baseUrl;
        protected HttpResponseMessage _responseMessage;
        protected string _responseBody = string.Empty;

        protected System.Net.Http.HttpClient _httpClient = null;
        HttpClientHandler _httpClientHandler = null;
        int TimeoutSecs = 120;

        int _appId;
        string _appName;
        #endregion

        /// <summary>
        /// Initiates
        /// </summary>
        public BaseHttpClient(Uri baseUrl)
        {
            SetupProtocol();
            if (baseUrl == null)
                throw new ArgumentNullException("baseUrl", "Provide base url for the constructor.");

            _httpClient = new System.Net.Http.HttpClient();
            _configuration = new ApplicationConfiguration();
            _appId = _configuration.GetConfigurationValue<int>("AppId");
            _appName = _configuration.GetConfigurationValue("AppName");
            _eventLogger = new EventLogger(_appName, _configuration);
            _baseUrl = baseUrl;

            _httpClient.Timeout = new TimeSpan(0, 0, TimeoutSecs);
        }

        /// <summary>
        /// Execute
        /// Get Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlKey"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual async Task<DomainResponse<T>> ExecuteAsync<T>(string appPath, HttpMethod method)
        {
            return await ExecuteHttpRequestAsync<T>(appPath, method, null, null, null, "");
        }

        public virtual DomainResponse<T> Execute<T>(string appPath, HttpMethod method)
        {
            return ExecuteHttpRequest<T>(appPath, method, null, null, null, "");
        }

        /// <summary>
        /// Execute
        /// Post or Put Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlKey"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public virtual async Task<DomainResponse<T>> ExecuteAsync<T>(string appPath, HttpMethod method, HttpContent content)
        {
            return await ExecuteHttpRequestAsync<T>(appPath, method, content, "");
        }

        public virtual DomainResponse<T> Execute<T>(string appPath, HttpMethod method, HttpContent content)
        {
            return ExecuteHttpRequest<T>(appPath, method, content, "");
        }

        /// <summary>
        /// Execute
        /// Get or Delete Method with Querystring Parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlKey"></param>
        /// <param name="method"></param>
        /// <param name="QueryStringVariables"></param>
        /// <returns></returns>
        public virtual async Task<DomainResponse<T>> ExecuteAsync<T>(string appPath, HttpMethod method, params string[] queryStringVariables)
        {
            return await ExecuteHttpRequestAsync<T>(appPath, method, null, queryStringVariables);
        }

        public virtual DomainResponse<T> Execute<T>(string appPath, HttpMethod method, params string[] queryStringVariables)
        {
            return ExecuteHttpRequest<T>(appPath, method, null, queryStringVariables);
        }

        /// <summary>
        /// Execute Http Request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="urlKey"></param>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <param name="queryStringVariables"></param>
        /// <returns></returns>
        private async Task<DomainResponse<T>> ExecuteHttpRequestAsync<T>(string appPath, HttpMethod method, HttpContent content, params string[] queryStringVariables)
        {
            if (string.IsNullOrEmpty(appPath))
                throw new ArgumentNullException("appPath", "Provide url for the request.");

            var urlValue = new Uri(_baseUrl.ToString().UriCombine(appPath));

            DomainResponse<T> response = null;
            HttpResponseMessage responseMessage = null;

            _httpClient.DefaultRequestHeaders.Clear();

            //set Timeout

            try
            {
                switch (method.Method)
                {
                    case "GET":
                        responseMessage = await _httpClient.GetAsync(string.Format(CultureInfo.InvariantCulture, urlValue.ToString(), queryStringVariables));
                        break;
                    case "POST":
                        responseMessage = await _httpClient.PostAsync(urlValue, content);
                        break;
                    case "PUT":
                        responseMessage = await _httpClient.PutAsync(urlValue, content);
                        break;
                    case "DELETE":
                        responseMessage = await _httpClient.DeleteAsync(string.Format(CultureInfo.InvariantCulture, urlValue.ToString(), queryStringVariables));
                        break;
                }
            }
            catch (AggregateException ex)
            {
                foreach (var item in ex.Flatten().InnerExceptions)
                {
                    if (item.GetType() == typeof(TaskCanceledException))
                    {
                        LogMessage eventLogMessage = new LogMessage()
                        {
                            RequestId = GuidFactory.Create(),
                            LogType = LogLevel.Error,
                            AppId = _appId,
                            AppName = _appName,
                            Message = string.Format("URL: {0} Reason: Timeout occured while calling API.", urlValue.ToString()),
                            StackTrace = string.Empty,
                            CreatedDateTime = DateTime.Now,
                            AppException = ex
                        };

                        _eventLogger.Log(eventLogMessage);

                        throw new Exception(string.Format("DEFAULT0001: Timeout occurred while calling API. Details: {0} Message."
                                                , ex.Message.ToString()));
                    }
                    else
                    {
                        LogMessage eventLogMessage = new LogMessage()
                        {
                            RequestId = GuidFactory.Create(),
                            LogType = LogLevel.Error,
                            AppId = _appId,
                            AppName = _appName,
                            Message = string.Format("URL: {0} Reason: BackEnd Server is not Responding.", urlValue.ToString()),
                            StackTrace = string.Empty,
                            CreatedDateTime = DateTime.Now,
                            AppException = ex
                        };

                        _eventLogger.Log(eventLogMessage);

                        throw new Exception(string.Format("DEFAULT0002: BackEnd Server is not Responding. Details: {0} Message."
                                                , ex.Message.ToString()));
                    }
                }
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                var responseContent = responseMessage.Content;
                _responseBody = await responseContent.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<T>(_responseBody);
                response = new DomainResponse<T>(result);
            }
            else
            {
                var errorResponse = await responseMessage.Content.ReadAsStringAsync();
                response = new DomainResponse<T>(new DomainError(responseMessage.StatusCode.ToString(), errorResponse));
                return response;
            }

            return response;
        }

        private DomainResponse<T> ExecuteHttpRequest<T>(string appPath, HttpMethod method, HttpContent content, params string[] queryStringVariables)
        {
            if (string.IsNullOrEmpty(appPath))
                throw new ArgumentNullException("appPath", "Provide url for the request.");

            var urlValue = new Uri(_baseUrl.ToString().UriCombine(appPath));

            DomainResponse<T> response = null;
            _httpClient.DefaultRequestHeaders.Clear();

            try
            {
                switch (method.Method)
                {
                    case "GET":
                        _responseMessage = _httpClient.GetAsync(string.Format(CultureInfo.InvariantCulture, urlValue.ToString(), queryStringVariables)).Result;
                        break;
                    case "POST":
                        _responseMessage = _httpClient.PostAsync(urlValue, content).Result;
                        break;
                    case "PUT":
                        _responseMessage = _httpClient.PutAsync(urlValue, content).Result;
                        break;
                    case "DELETE":
                        _responseMessage = _httpClient.DeleteAsync(string.Format(CultureInfo.InvariantCulture, urlValue.ToString(), queryStringVariables)).Result;
                        break;
                }
            }
            catch (AggregateException ex)
            {
                foreach (var item in ex.Flatten().InnerExceptions)
                {
                    if (item.GetType() == typeof(TaskCanceledException))
                    {
                        LogMessage eventLogMessage = new LogMessage()
                        {
                            RequestId = GuidFactory.Create(),
                            LogType = LogLevel.Error,
                            AppId = _appId,
                            AppName = _appName,
                            Message = string.Format("URL: {0} Reason: Timeout occured while calling API.", urlValue.ToString()),
                            StackTrace = string.Empty,
                            CreatedDateTime = DateTime.Now,
                            AppException = ex
                        };

                        _eventLogger.Log(eventLogMessage);

                        throw new Exception(string.Format("DEFAULT0001: Timeout occurred while calling API. Details: {0} Message."
                                                , ex.Message.ToString()));
                    }
                    else
                    {
                        LogMessage eventLogMessage = new LogMessage()
                        {
                            RequestId = GuidFactory.Create(),
                            LogType = LogLevel.Error,
                            AppId = _appId,
                            AppName = _appName,
                            Message = string.Format("URL: {0} Reason: BackEnd Server is not Responding.", urlValue.ToString()),
                            StackTrace = string.Empty,
                            CreatedDateTime = DateTime.Now,
                            AppException = ex
                        };

                        _eventLogger.Log(eventLogMessage);

                        throw new Exception(string.Format("DEFAULT0002: BackEnd Server is not Responding. Details: {0} Message."
                                                , ex.Message.ToString()));
                    }
                }
            }

            if (_responseMessage.IsSuccessStatusCode)
            {
                var responseContent = _responseMessage.Content;
                _responseBody = responseContent.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<T>(_responseBody);
                response = new DomainResponse<T>(result);
            }
            else
            {
                var errorResponse = _responseMessage.Content.ReadAsStringAsync().Result;
                response = new DomainResponse<T>(new DomainError(_responseMessage.StatusCode.ToString(), errorResponse));
                return response;
            }

            return response;
        }

        private void SetupProtocol()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _httpClientHandler.Dispose();
                    _httpClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~HttpClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
