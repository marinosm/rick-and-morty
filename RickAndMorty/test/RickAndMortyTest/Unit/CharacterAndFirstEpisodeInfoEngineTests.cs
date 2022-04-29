using Microsoft.Extensions.Logging.Abstractions;
using RickAndMortyApiClient;
using RickAndMortyEngine;
using RickAndMortyEngineDefault;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace RickAndMortyTest.Unit
{
    public class CharacterAndFirstEpisodeInfoEngineTests
    {
        private static readonly IList<CharacterDto> _characters = new List<CharacterDto>
        {
            new CharacterDto
            {
                Id = 1, Name = "Rick 1", Url = "character/1",
                Episode = new string[] { "episode/100", "episode/200" }
            },
            new CharacterDto
            {
                Id = 2, Name = "Rick 2", Url = "character/2",
                Episode = new string[] { "episode/200" },
            },
            new CharacterDto
            {
                Id = 3, Name = "Morty", Url = "character/3",
                Episode = new string[] { "episode/100", "episode/200" }
            },
            new CharacterDto
            {
                Id = 4, Name = "Other", Url = "character/4",
                Episode = new string[] { "episode/400" }
            }
        };

        private readonly IList<LocationDto> _locations = new List<LocationDto> { };

        private readonly IList<EpisodeDto> _episodes = new List<EpisodeDto>
        {
            new EpisodeDto
            {
                Id = 100, Url = "episode/100", Air_date = new DateTime(2000, 01, 01).ToString("MMMM d, yyyy", CultureInfo.InvariantCulture),
                Characters = new string[] { "character/1", "character/3" }
            },
            new EpisodeDto
            {
                Id = 200, Url = "episode/200", Air_date = new DateTime(2000, 02, 02).ToString("MMMM d, yyyy", CultureInfo.InvariantCulture),
                Characters = new string[] { "character/1", "character/2", "character/3" }
            },
            new EpisodeDto
            {
                Id = 400, Url = "episode/400", Air_date = new DateTime(2000, 04, 04).ToString("MMMM d, yyyy", CultureInfo.InvariantCulture),
                Characters = new string[] { "character/4" }
            }
        };

        private readonly ICharacterAndFirstEpisodeInfoEngine _engine;

        public CharacterAndFirstEpisodeInfoEngineTests()
        {
            _engine = new CharacterAndFirstEpisodeInfoEngine(
                new NullLogger<CharacterAndFirstEpisodeInfoEngine>(),
                new MockClient(_characters, _locations, _episodes));
        }

        public static IEnumerable<object[]> CharacterAndFirstEpisodeInfoEngineTestData =>
            new List<object[]>
            {
                new object[] { new FindCharactersRequest { Name = "rick 2" }, new List<(int, string, List<string>)> { (2, "episode/200", new List<string> { "character/2" }) }},
                new object[] { new FindCharactersRequest { Name = "rick 1" }, new List<(int, string, List<string>)> { (1, "episode/100", new List<string> { "character/1", "character/3" }) } },
                new object[] { new FindCharactersRequest { Name = "morty" },  new List<(int, string, List<string>)> { (3, "episode/100", new List<string> { "character/1", "character/3" }) } },
                new object[] { new FindCharactersRequest { Name = "rick" }, new List<(int, string, List<string>)> { (1, "episode/100", new List<string> { "character/1", "character/3" }), (2, "episode/200", new List<string> { "character/2" }) } }
            };

        [Theory]
        [MemberData(nameof(CharacterAndFirstEpisodeInfoEngineTestData))]
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
