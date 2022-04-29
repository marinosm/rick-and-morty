using System;
using System.Net.Http;

namespace RickAndMortyTest.Integration
{
    internal class DefaultHttpClientFactory : IHttpClientFactory
    {
        private readonly string _baseAddress;
        private readonly TimeSpan _timeout;

        public DefaultHttpClientFactory(string baseAddress, TimeSpan timeout)
        {
            _baseAddress = baseAddress ?? throw new ArgumentNullException(nameof(baseAddress));
            _timeout = timeout;
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient
            {
                BaseAddress = new Uri(_baseAddress),
                Timeout = _timeout
            };
        }
    }
}
