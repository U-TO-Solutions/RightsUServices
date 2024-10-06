namespace UTO.Framework.Shared.Models.Domain
{
    /// <summary>
    /// Represents violation of a domain validation or rule.
    /// </summary>
    public class DomainError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainError"/> class.
        /// </summary>
        /// <param name="errorCode">The error code identifier.</param>
        /// <param name="message">The message.</param>
        public DomainError(string errorCode, string message)
        {
            this.ErrorCode = errorCode;
            this.Message = message;
        }

        /// <summary>
        /// Gets the error code identifier.
        /// </summary>
        /// <value>
        /// The error code identifier.
        /// </value>
        public string ErrorCode { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; private set; }
    }
}
