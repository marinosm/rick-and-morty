using System.Threading.Tasks;

namespace RickAndMortyEngine
{
    public interface ICharacterAndCoresidentsEngine
    {
        Task<EngineResponse<Page<CharacterAndCoresidents>>> FindCharactersAndCoresidents(FindCharactersRequest request);
    }
}
