using RickAndMortyApiClient;

namespace RickAndMortyEngineDefault
{
    internal class CharacterInfo
    {
        internal readonly CharacterDto Dto;
        
        private string _earliestEpisodeUrl;

        public CharacterInfo(CharacterDto dto)
        {
            Dto = dto;
        }

        public void SetEarliestEpisodeUrl(string episodeUrl)
        {
            _earliestEpisodeUrl = episodeUrl;
        }

        public string GetEarliestEpisodeUrl()
        {
            return _earliestEpisodeUrl;
        }
    }
}
