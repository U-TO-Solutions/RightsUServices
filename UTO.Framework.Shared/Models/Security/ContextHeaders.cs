using System.Web;
using System.Collections.Specialized;

namespace UTO.Framework.Shared.Models.Security
{
    public class ContextHeaders
    {
        private const string AppReqContext = "AppReqContext";
        public ContextHeaders()
        {
            var context = HttpContext.Current != null ? (ContextHeaders)HttpContext.Current.Items[AppReqContext] : null;
            if (context != null)
            {
                this.AccessContext = context.AccessContext;
                this.RequestContext = context.RequestContext;
            }
            else
            {
                this.AccessContext = new AccessContext();
                this.RequestContext = new RequestContext();
            }

        }
        /// <summary>
        /// Gets or Sets Access Context
        /// </summary>
        public AccessContext AccessContext { get; set; }

        /// <summary>
        /// Gets or Sets Request Context
        /// </summary>
        public RequestContext RequestContext { get; set; }
    }
}
