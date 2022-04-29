using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RickAndMortyApiClient;
using RickAndMortyApiClientDefault;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RickAndMortyApiClientTest
{
    public class FilterCharactersTests
    {
        private readonly IClient _client;

        public FilterCharactersTests()
        {
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory
                .Setup(_ => _.CreateClient("default"))
                .Returns(new HttpClient
                {
                    BaseAddress = new Uri("https://rickandmortyapi.com/api/"),
                    Timeout = TimeSpan.FromSeconds(5)
                });

            _client = new Client(new NullLogger<Client>(), mockHttpClientFactory.Object);
        }

        [Fact]
        public async Task FilterCharacters_Null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => { await _client.FilterCharacters(null); });
        }

        [Fact]
        public async Task FilterCharacters_Unknown()
        {
            var request = new CharacterFilters
            {
                Name = "this name does not exist"
            };
            var response = await _client.FilterCharacters(request);
            Assert.Equal(HttpStatusCode.NotFound, response.ApiResponseCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("rick")]
        public async Task FilterCharacters_Valid(string name)
        {
            var request = new CharacterFilters
            {
                Name = name
            };
            var response = await _client.FilterCharacters(request);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.ApiResponseCode);

            var responseData = response.Data;

            Assert.NotNull(responseData);
            Assert.NotNull(responseData.Info);
            Assert.NotNull(responseData.Results);
            Assert.NotEmpty(responseData.Results);
        }


    }
}
