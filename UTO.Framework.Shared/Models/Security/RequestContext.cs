using System;

namespace UTO.Framework.Shared.Models.Security
{
    public class RequestContext
    {
        /// <summary>
        /// Gets or Sets Application Id
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Gets or Sets Application Name
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or Sets Request ID
        /// </summary>
        public string RequestID { get; set; }

        /// <summary>
        /// Gets or Sets Client IP Address
        /// </summary>
        public string ClientIPAddress { get; set; }

        /// <summary>
        /// Gets or Sets Request Date Time
        /// </summary>
        public DateTime RequestDateTime { get; set; }

        /// <summary>
        /// Gets or Sets Web Server Name
        /// </summary>
        public string WebServerName { get; set; }

        /// <summary>
        /// Gets or Sets App Server Name
        /// </summary>
        public string AppServerName { get; set; }
    }
}
