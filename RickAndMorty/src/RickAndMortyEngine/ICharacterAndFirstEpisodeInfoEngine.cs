using System.Threading.Tasks;

namespace RickAndMortyEngine
{
    public interface ICharacterAndFirstEpisodeInfoEngine
    {
        Task<EngineResponse<Page<CharacterAndFirstEpisodeInfo>>> FindCharactersAndFirstEpisodeInfo(FindCharactersRequest request);
    }
}
