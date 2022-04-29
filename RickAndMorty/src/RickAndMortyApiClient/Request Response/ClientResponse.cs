using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace RickAndMortyApiClient
{
    [ExcludeFromCodeCoverage]
    public class ClientResponse<T>
    {
        public HttpStatusCode ApiResponseCode { get; set; }
        public string ApiResponseMessage { get; set; }
        public T Data { get; set; }
    }
}
