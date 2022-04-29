using System.Collections.Generic;
using System.Threading.Tasks;

namespace RickAndMortyApiClient
{
    public interface IClient
    {
        Task<ClientResponse<PageDto<CharacterDto>>> FilterCharacters(CharacterFilters filters);
        Task<ClientResponse<List<CharacterDto>>> GetMultipleCharacters(List<string> ids);
        Task<ClientResponse<List<LocationDto>>> GetMultipleLocations(List<string> ids);
        Task<ClientResponse<List<EpisodeDto>>> GetMultipleEpisodes(List<string> ids);
    }
}
