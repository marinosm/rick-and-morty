using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace RickAndMortyEngine
{
    [ExcludeFromCodeCoverage]
    public class EngineResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public T Data { get; set; }
    }
}
