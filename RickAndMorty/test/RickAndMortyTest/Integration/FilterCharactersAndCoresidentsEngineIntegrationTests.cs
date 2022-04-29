using Microsoft.Extensions.Logging.Abstractions;
using RickAndMortyApiClientDefault;
using RickAndMortyEngine;
using RickAndMortyEngineDefault;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace RickAndMortyTest.Integration
{
    public class FilterCharactersAndCoresidentsEngineIntegrationTests
    {
        private readonly ICharacterAndCoresidentsEngine _engine;

        public FilterCharactersAndCoresidentsEngineIntegrationTests()
        {
            var httpClientFactory = new DefaultHttpClientFactory(
                "https://rickandmortyapi.com/api/",
                TimeSpan.FromSeconds(5));

            _engine = new CharacterAndCoresidentsEngine(
                new NullLogger<CharacterAndCoresidentsEngine>(),
                new Client(new NullLogger<Client>(), httpClientFactory));
        }

        public static IEnumerable<object[]> CharacterAndCoresidentsEngineIntegrationTestData =>
            new List<object[]>
            {
                new object[] {
                    new FindCharactersRequest { Name = "Alan Rails" },
                    new List<(int, List<string>)> { (
                        10,
                        new List<string>
                        {
                            "https://rickandmortyapi.com/api/character/10",
                            "https://rickandmortyapi.com/api/character/81",
                            "https://rickandmortyapi.com/api/character/208",
                            "https://rickandmortyapi.com/api/character/226",
                            "https://rickandmortyapi.com/api/character/340",
                            "https://rickandmortyapi.com/api/character/362",
                            "https://rickandmortyapi.com/api/character/375",
                            "https://rickandmortyapi.com/api/character/382",
                            "https://rickandmortyapi.com/api/character/395" 
                        }) }}
            };

        [Theory]
        [MemberData(nameof(CharacterAndCoresidentsEngineIntegrationTestData))]
        public async Task Test(FindCharactersRequest request, List<(int, List<string>)> expectedResults)
        {
            var response = await _engine.FindCharactersAndCoresidents(request);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);
            var responsePage = response.Data;
            Assert.NotNull(responsePage.Data);
            var results = responsePage.Data;

            Assert.Equal(expectedResults.Count, results.Count);

            foreach (var (expectedId, expectedResidents) in expectedResults)
            {
                Assert.Contains(results, r => r.Id == expectedId);

                var result = results.Find(r => r.Id == expectedId);
                Assert.Equal(expectedResidents.Count, result.OtherCharactersInLocation.Length);

                foreach (var coresident in expectedResidents)
                {
                    Assert.Contains(result.OtherCharactersInLocation, r => r == coresident);
                }
            }
        }
    }
}
