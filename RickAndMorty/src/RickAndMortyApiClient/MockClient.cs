using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RickAndMortyApiClient
{
    public class MockClient : IClient
    {
        private readonly IList<CharacterDto> _characters;
        private readonly IList<LocationDto> _locations;
        private readonly IList<EpisodeDto> _episodes;
        private readonly int _itemsPerPage;

        public MockClient(
            IList<CharacterDto> characters,
            IList<LocationDto> locations,
            IList<EpisodeDto> episodes,
            int itemsPerPage = 20)
        {
            _characters = characters ?? throw new ArgumentNullException(nameof(characters));
            _locations = locations ?? throw new ArgumentNullException(nameof(locations));
            _episodes = episodes ?? throw new ArgumentNullException(nameof(episodes));
            _itemsPerPage = itemsPerPage;
        }

        private int GetPageCount(int resultsCount) => resultsCount / _itemsPerPage + (resultsCount % _itemsPerPage > 0 ? 1 : 0);

        private PageInfoDto GetPageInfo(int resultsCount) => new PageInfoDto { Count = resultsCount, Pages = GetPageCount(resultsCount) };

        public Task<ClientResponse<PageDto<CharacterDto>>> FilterCharacters(CharacterFilters filters)
        {
            if (filters is null) throw new ArgumentNullException(nameof(filters));

            var results = _characters
                .Where(character =>
                    (filters.Name is null || character.Name.ToLower().Contains(filters.Name.ToLower()))
                    &&
                    (filters.Type is null || character.Type.ToLower().Contains(filters.Type?.ToLower()))
                    &&
                    (filters.Gender is null || character.Gender.ToLower().Contains(filters.Gender?.ToLower()))
                    &&
                    (filters.Species is null || character.Species.ToLower().Contains(filters.Species?.ToLower()))
                    &&
                    (filters.Status is null || character.Status.ToLower().Contains(filters.Status?.ToLower())));

            return Task.FromResult(new ClientResponse<PageDto<CharacterDto>>
            {
                ApiResponseCode = HttpStatusCode.OK,
                ApiResponseMessage = string.Empty,
                Data = new PageDto<CharacterDto>
                {
                    Info = GetPageInfo(results.Count()),
                    Results = results
                                .Skip(_itemsPerPage * ((filters.Page ?? 1) - 1))
                                .Take(_itemsPerPage)
                                .ToList()
                }
            });
        }

        public Task<ClientResponse<List<CharacterDto>>> GetMultipleCharacters(List<string> ids)
        {
            var results = _characters
                .Where(character => ids.Contains(
                    character.Id.ToString()))
                .ToList();

            return Task.FromResult(new ClientResponse<List<CharacterDto>>
            {
                ApiResponseCode = HttpStatusCode.OK,
                ApiResponseMessage = string.Empty,
                Data = results
            });
        }

        public Task<ClientResponse<List<EpisodeDto>>> GetMultipleEpisodes(List<string> ids)
        {
            var results = _episodes
                .Where(episode => ids.Contains(
                    episode.Id.ToString()))
                .ToList();

            return Task.FromResult(new ClientResponse<List<EpisodeDto>>
            {
                ApiResponseCode = HttpStatusCode.OK,
                ApiResponseMessage = string.Empty,
                Data = results
            });
        }

        public Task<ClientResponse<List<LocationDto>>> GetMultipleLocations(List<string> ids)
        {
            var results = _locations
                .Where(location => ids.Contains(
                    location.Id.ToString()))
                .ToList();

            return Task.FromResult(new ClientResponse<List<LocationDto>>
            {
                ApiResponseCode = HttpStatusCode.OK,
                ApiResponseMessage = string.Empty,
                Data = results
            });
        }
    }
}
