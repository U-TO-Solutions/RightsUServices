using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using UTO.Framework.Shared.Models.Security;
using UTO.Framework.Shared.Resources;
using UTO.Framework.SharedInfrastructure.Repository;
using data = UTO.Framework.SharedInfrastructure.Data;

namespace UTO.Framework.SharedInfrastructure.Network
{
    public class TraceHandler : DelegatingHandler
    {
        private readonly TraceRepository traceRepository = new TraceRepository();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            ContextHeaders context = null;

            if (HttpContext.Current.Items[AppConstants.AppReqContext] != null)
            {
                context = (ContextHeaders)HttpContext.Current.Items[AppConstants.AppReqContext];
            }

            HttpTrace trace = new HttpTrace(context.RequestContext.RequestID, request);
            var requestContent = request.Content.ReadAsStringAsync().Result;
            trace.RequestContent = requestContent;
            var response = base.SendAsync(request, cancellationToken);
            trace.AddResponse(response.Result);

            string UsersCode = "0";

            if (request.Headers.Contains("userCode"))
                UsersCode = Convert.ToString(request.Headers.GetValues("userCode").FirstOrDefault());
            UsersCode = UsersCode.Replace("Bearer ", "");

            if (trace.RequestMethod == "Login/GetLoginDetails" && trace.Method == "POST")
            {
                dynamic loginResponseContent = JsonConvert.DeserializeObject<dynamic>(trace.ResponseContent);
                if (loginResponseContent.User != null)
                    UsersCode = loginResponseContent.User.Users_Code;
            }

            if (trace.Method != "OPTIONS")
            {
                data.Trace traceEntity = new data.Trace();

                if (trace.Authenticationkey != null && !string.IsNullOrEmpty(trace.Authenticationkey))
                    traceEntity.AuthenticationKey = trace.Authenticationkey;
                traceEntity.HttpStatusCode = (int)trace.StatusCode;
                traceEntity.HttpStatusDescription = trace.StatusCode.ToString();
                traceEntity.IsSuccess = trace.IsSuccess;
                traceEntity.Method = trace.Method;
                traceEntity.RequestContent = trace.RequestContent;
                traceEntity.RequestDateTime = trace.RequestDateTime;
                traceEntity.RequestId = trace.RequestId;
                traceEntity.RequestLength = trace.RequestLength;
                traceEntity.RequestUri = trace.RequestUri;
                traceEntity.RequestMethod = trace.RequestMethod;
                traceEntity.ResponseContent = trace.ResponseContent;
                traceEntity.ResponseDateTime = trace.ResponseDateTime;
                traceEntity.ResponseLength = trace.ResponseLength;
                traceEntity.ServerName = trace.ServerName;
                traceEntity.ClientIpAddress = trace.ClientIpAddress;
                traceEntity.TimeTaken = trace.TimeTaken;
                traceEntity.UserAgent = trace.UserAgent;
                traceEntity.UserId = Convert.ToInt32(UsersCode);

                traceRepository.AddEntity(traceEntity);
            }

            return response;
        }
    }
}
