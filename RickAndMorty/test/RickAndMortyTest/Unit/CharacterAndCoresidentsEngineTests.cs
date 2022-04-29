using Microsoft.Extensions.Logging.Abstractions;
using RickAndMortyApiClient;
using RickAndMortyEngine;
using RickAndMortyEngineDefault;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace RickAndMortyTest.Unit
{
    public class CharacterAndCoresidentsEngineTests
    {
        private static readonly IList<CharacterDto> _characters = new List<CharacterDto>
        {
            new CharacterDto
            {
                Id = 1, Name = "Rick 1", Url = "character/1",
                Location = new CharacterLocationDto { Url = "location/10" }
            },
            new CharacterDto
            {
                Id = 2, Name = "Rick 2", Url = "character/2",
                Location = new CharacterLocationDto { Url = "location/20" }
            },
            new CharacterDto
            {
                Id = 3, Name = "Morty", Url = "character/3",
                Location = new CharacterLocationDto { Url = "location/10" }
            },
            new CharacterDto
            {
                Id = 4, Name = "Other", Url = "character/4",
                Location = new CharacterLocationDto { Url = "location/10" }
            }
        };

        private readonly IList<LocationDto> _locations = new List<LocationDto>
        {
            new LocationDto
            {
                Id = 10, Url = "location/10",
                Residents = new string[] { "character/1", "character/3", "character/4" }
            },
            new LocationDto
            {
                Id = 20, Url = "location/20",
                Residents = new string[] { "character/2" }
            }
        };

        private readonly IList<EpisodeDto> _episodes = new List<EpisodeDto> { };

        private readonly ICharacterAndCoresidentsEngine _engine;

        public CharacterAndCoresidentsEngineTests()
        {
            _engine = new CharacterAndCoresidentsEngine(
                new NullLogger<CharacterAndCoresidentsEngine>(),
                new MockClient(_characters, _locations, _episodes));
        }

        public static IEnumerable<object[]> CharacterAndCoresidentsEngineTestData =>
            new List<object[]>
            {
                new object[] { new FindCharactersRequest { Name = "rick 2" }, new List<(int, List<string>)> { (2, new List<string> { "character/2" }) }},
                new object[] { new FindCharactersRequest { Name = "rick 1" }, new List<(int, List<string>)> { (1, new List<string> { "character/1", "character/3", "character/4" }) } },
                new object[] { new FindCharactersRequest { Name = "morty" }, new List<(int, List<string>)> { (3, new List<string> { "character/1", "character/3", "character/4" }) } },
                new object[] { new FindCharactersRequest { Name = "rick" }, new List<(int, List<string>)> { (1, new List<string> { "character/1", "character/3", "character/4" }), (2, new List<string> { "character/2" }) } }
            };

        [Theory]
        [MemberData(nameof(CharacterAndCoresidentsEngineTestData))]
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
