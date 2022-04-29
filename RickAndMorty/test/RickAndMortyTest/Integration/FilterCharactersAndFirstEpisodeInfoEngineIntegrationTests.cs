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
    public class CharacterAndFirstEpisodeInfoEngineIntegrationTests
    {
        private readonly ICharacterAndFirstEpisodeInfoEngine _engine;

        public CharacterAndFirstEpisodeInfoEngineIntegrationTests()
        {
            var httpClientFactory = new DefaultHttpClientFactory(
                "https://rickandmortyapi.com/api/",
                TimeSpan.FromSeconds(5));

            _engine = new CharacterAndFirstEpisodeInfoEngine(
                new NullLogger<CharacterAndFirstEpisodeInfoEngine>(),
                new Client(new NullLogger<Client>(), httpClientFactory));
        }

        public static IEnumerable<object[]> CharacterAndFirstEpisodeInfoEngineIntegrationTestData =>
            new List<object[]>
            {
                new object[] {
                    new FindCharactersRequest
                    {
                        Name = "Rick Sanchez",  Status = "Alive", Species = "Human", Type = "", Gender = "Male", Page = null
                    },
                    new List<(int, string, List<string>)>
                    {
                        (1, "https://rickandmortyapi.com/api/episode/1", 
                            new List<string>
                            {
                                "https://rickandmortyapi.com/api/character/1",
                                "https://rickandmortyapi.com/api/character/2",
                                "https://rickandmortyapi.com/api/character/35",
                                "https://rickandmortyapi.com/api/character/38",
                                "https://rickandmortyapi.com/api/character/62",
                                "https://rickandmortyapi.com/api/character/92",
                                "https://rickandmortyapi.com/api/character/127",
                                "https://rickandmortyapi.com/api/character/144",
                                "https://rickandmortyapi.com/api/character/158",
                                "https://rickandmortyapi.com/api/character/175",
                                "https://rickandmortyapi.com/api/character/179",
                                "https://rickandmortyapi.com/api/character/181",
                                "https://rickandmortyapi.com/api/character/239",
                                "https://rickandmortyapi.com/api/character/249",
                                "https://rickandmortyapi.com/api/character/271",
                                "https://rickandmortyapi.com/api/character/338",
                                "https://rickandmortyapi.com/api/character/394",
                                "https://rickandmortyapi.com/api/character/395",
                                "https://rickandmortyapi.com/api/character/435"
                            }),
                        (631, "https://rickandmortyapi.com/api/episode/37",
                            new List<string>
                            {
                                "https://rickandmortyapi.com/api/character/631",
                                "https://rickandmortyapi.com/api/character/593",
                                "https://rickandmortyapi.com/api/character/594",
                                "https://rickandmortyapi.com/api/character/595",
                                "https://rickandmortyapi.com/api/character/596",
                                "https://rickandmortyapi.com/api/character/597",
                                "https://rickandmortyapi.com/api/character/598",
                                "https://rickandmortyapi.com/api/character/599",
                                "https://rickandmortyapi.com/api/character/600",
                                "https://rickandmortyapi.com/api/character/601",
                                "https://rickandmortyapi.com/api/character/602",
                                "https://rickandmortyapi.com/api/character/603",
                                "https://rickandmortyapi.com/api/character/604",
                                "https://rickandmortyapi.com/api/character/605",
                                "https://rickandmortyapi.com/api/character/606",
                                "https://rickandmortyapi.com/api/character/607",
                                "https://rickandmortyapi.com/api/character/608",
                                "https://rickandmortyapi.com/api/character/609",
                                "https://rickandmortyapi.com/api/character/610",
                                "https://rickandmortyapi.com/api/character/611",
                                "https://rickandmortyapi.com/api/character/612",
                                "https://rickandmortyapi.com/api/character/613",
                                "https://rickandmortyapi.com/api/character/614",
                                "https://rickandmortyapi.com/api/character/615",
                                "https://rickandmortyapi.com/api/character/616",
                                "https://rickandmortyapi.com/api/character/617",
                                "https://rickandmortyapi.com/api/character/618",
                                "https://rickandmortyapi.com/api/character/619",
                                "https://rickandmortyapi.com/api/character/620",
                                "https://rickandmortyapi.com/api/character/621",
                                "https://rickandmortyapi.com/api/character/622",
                                "https://rickandmortyapi.com/api/character/623",
                                "https://rickandmortyapi.com/api/character/624",
                                "https://rickandmortyapi.com/api/character/625",
                                "https://rickandmortyapi.com/api/character/626",
                                "https://rickandmortyapi.com/api/character/627",
                                "https://rickandmortyapi.com/api/character/628",
                                "https://rickandmortyapi.com/api/character/629",
                                "https://rickandmortyapi.com/api/character/630",
                                "https://rickandmortyapi.com/api/character/632",
                                "https://rickandmortyapi.com/api/character/633",
                                "https://rickandmortyapi.com/api/character/634",
                                "https://rickandmortyapi.com/api/character/635",
                                "https://rickandmortyapi.com/api/character/636",
                                "https://rickandmortyapi.com/api/character/637",
                                "https://rickandmortyapi.com/api/character/638",
                                "https://rickandmortyapi.com/api/character/639"
                            })
                    }
                }
            };

        [Theory]
        [MemberData(nameof(CharacterAndFirstEpisodeInfoEngineIntegrationTestData))]
        public async Task Test(FindCharactersRequest request, List<(int, string, List<string>)> expectedResults)
        {
            var response = await _engine.FindCharactersAndFirstEpisodeInfo(request);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);
            var responsePage = response.Data;
            Assert.NotNull(responsePage.Data);
            var results = responsePage.Data;

            Assert.Equal(expectedResults.Count, results.Count);

            foreach (var (expectedId, expectedFirstEpisode, expectedCharactersFirstSeen) in expectedResults)
            {
                Assert.Contains(results, r => r.Id == expectedId);

                var result = results.Find(r => r.Id == expectedId);
                Assert.Equal(expectedFirstEpisode, result.FirstSeenInEpisode);
                Assert.Equal(expectedCharactersFirstSeen.Count, result.OtherCharactersFirstSeenInTheEpisode.Length);

                foreach (var firstSeenCharater in expectedCharactersFirstSeen)
                {
                    Assert.Contains(result.OtherCharactersFirstSeenInTheEpisode, r => r == firstSeenCharater);
                }
            }
        }
    }
}
