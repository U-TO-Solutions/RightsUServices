using System.Collections.Generic;
using System.Linq;

namespace UTO.Framework.Shared.Models.Domain
{
    /// <summary>
    /// Represents base class for Domain response.
    /// </summary>
    /// <typeparam name="T">
    /// Concrete type to represent result of a Domain operation.
    /// </typeparam>
    public class DomainResponse<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainResponse{T}"/> class.
        /// </summary>
        /// <param name="errors">
        /// The errors.
        /// </param>
        public DomainResponse(IList<DomainError> errors)
        {
            this.Errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainResponse{T}"/> class.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        public DomainResponse(DomainError error)
        {
            this.Errors = new List<DomainError>() { error };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainResponse{T}"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public DomainResponse(T result)
        {
            this.Result = result;
        }

        public DomainResponse()
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors
        {
            get
            {
                return this.Errors != null && this.Errors.Count > 0;
            }
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public IList<DomainError> Errors { get; private set; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public T Result { get; private set; }

        /// <summary>
        /// Add Domain Error
        /// </summary>
        /// <param name="error">Domain Error Object</param>
        public void AddError(DomainError error)
        {
            if (this.Errors == null)
            {
                this.Errors = new List<DomainError>() { error };
            }
            else
            {
                this.Errors.Add(error);
            }
        }

        /// <summary>
        /// Add Error
        /// </summary>
        /// <param name="errors"></param>
        public void AddError(IList<DomainError> errors)
        {
            this.Errors = errors;
        }

        /// <summary>
        /// To Error String
        /// </summary>
        /// <returns></returns>
        public string ToErrorString()
        {
            if (this.HasErrors)
                return string.Join(",", this.Errors.ToList().Select(p => p.ErrorCode + ":" + p.Message));
            else
                return string.Empty;
        }
    }
}
