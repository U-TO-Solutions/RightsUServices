using System;
using System.Net;
using System.Net.Http;
using UTO.Framework.SharedInfrastructure.Extensions;

namespace UTO.Framework.SharedInfrastructure.Network
{
    public class HttpTrace
    {
        #region private class members
        private const string X_FORWARDED_FOR = "X-FORWARDED-FOR";
        private const string REMOTE_ADDR = "REMOTE_ADDR";

        private readonly string _requestId;
        private readonly DateTime _requestDateTime;
        private readonly HttpRequestMessage _httpRequest;
        private HttpResponseMessage _httpResponse;
        private DateTime _responseDateTime;

        private bool _isComplete;
        #endregion

        #region constructor
        public HttpTrace(string id, HttpRequestMessage requestMessage)
        {
            Shared.Guards.ParameterGuard.AgainstNullStringParameter(id);
            Shared.Guards.ParameterGuard.AgainstNullParameter(requestMessage);
            _isComplete = false;
            _requestId = id;
            _requestDateTime = DateTime.Now;
            _httpRequest = requestMessage;
        }
        #endregion

        #region properties
        public bool IsComplete => _isComplete;
        public string RequestId => this._requestId;
        public bool IsSuccess => _isComplete ? _httpResponse.IsSuccessStatusCode : false;
        public int RequestLength => (_httpRequest.Method == HttpMethod.Post) ? _httpRequest.Content.ReadAsStringAsync().Result.Length : 0;
        public int ResponseLength => (IsComplete && _httpResponse.Content != null) ? _httpResponse.Content.ReadAsStringAsync().Result.Length : 0;
        public string ResponseContent => (IsComplete) ? (_httpResponse.Content != null) ? _httpResponse.Content.ReadAsStringAsync().Result : string.Empty : string.Empty;
        //public string RequestContent => (_httpRequest.Method == HttpMethod.Post) ? _httpRequest.Content.ReadAsStringAsync().Result : string.Empty;
        public string RequestContent { get; set; }
        public HttpStatusCode StatusCode => _httpResponse.StatusCode;
        public string Authenticationkey => (_httpRequest.Headers.Authorization != null) ? _httpRequest.Headers.Authorization.ToString() : string.Empty;
        public string UserAgent => _httpRequest.Headers.UserAgent.ToString();
        public string ServerName => System.Environment.MachineName;
        public string ClientIpAddress => _httpRequest.GetClientIpAddress();
        public string Method => _httpRequest.Method.Method.ToString();
        public string RequestUri => _httpRequest.RequestUri.AbsolutePath.ToString();
        public string RequestMethod => (_httpRequest.RequestUri.AbsolutePath.ToString() != null) ? (_httpRequest.RequestUri.AbsolutePath.ToString().IndexOf("/api/") != -1) ? _httpRequest.RequestUri.AbsolutePath.ToString().Substring(_httpRequest.RequestUri.AbsolutePath.ToString().IndexOf("/api/") + 5) : _httpRequest.RequestUri.AbsolutePath.ToString() : string.Empty;
        public int TimeTaken => (IsComplete) ? _responseDateTime.Subtract(_requestDateTime).Milliseconds : 0;
        public DateTime RequestDateTime => _requestDateTime;
        public DateTime ResponseDateTime
        {
            get
            {
                if (IsComplete)
                {
                    return _responseDateTime;
                }
                else
                {
                    throw new InvalidOperationException("Response not received");
                }
            }
        }
        #endregion

        #region public methods
        public void AddResponse(HttpResponseMessage responseMessage)
        {
            Shared.Guards.ParameterGuard.AgainstNullParameter(responseMessage);
            _httpResponse = responseMessage;
            _responseDateTime = DateTime.Now;
            _isComplete = true;
        }
        #endregion

        #region private methods
        private string ExtractIPAddress(HttpRequestMessage requestMessage)
        {
            if (requestMessage.Headers.Contains(X_FORWARDED_FOR))
                return requestMessage.Headers.GetValues(X_FORWARDED_FOR).ToString();
            else
                return requestMessage.Headers.GetValues(REMOTE_ADDR).ToString();
        }
        #endregion
    }
}
