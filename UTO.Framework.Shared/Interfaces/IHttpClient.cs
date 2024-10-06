using System.Net.Http;
using System.Threading.Tasks;
using UTO.Framework.Shared.Models.Domain;

namespace UTO.Framework.Shared.Interfaces
{
    public interface IHttpClient
    {
        DomainResponse<T> Execute<T>(string appPath, HttpMethod method);
        DomainResponse<T> Execute<T>(string appPath, HttpMethod method, HttpContent content);
        DomainResponse<T> Execute<T>(string appPath, HttpMethod method, params string[] queryStringVariables);
        Task<DomainResponse<T>> ExecuteAsync<T>(string appPath, HttpMethod method);
        Task<DomainResponse<T>> ExecuteAsync<T>(string appPath, HttpMethod method, params string[] queryStringVariables);
        Task<DomainResponse<T>> ExecuteAsync<T>(string appPath, HttpMethod method, HttpContent content);
    }
}
