using RickAndMortyApiClient;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace RickAndMortyEngineDefault
{
    internal class EpisodeInfo
    {
        internal readonly DateTime AirDate;
        internal readonly string[] CastUrls;

        private readonly HashSet<string> _characterUrlsFirstSeenInThisEpisode;

        public EpisodeInfo(EpisodeDto dto)
        {
            if (string.IsNullOrEmpty(dto.Air_date))
            {
                AirDate = DateTime.MaxValue;
            }
            else
            {
                // Use ParseExact instead of TryParseExact
                // If we fail to parse a non empty date then 
                // better throw instead of returning false results
                AirDate = DateTime.ParseExact(dto.Air_date, "MMMM d, yyyy", CultureInfo.InvariantCulture);
            }
            CastUrls = dto.Characters;

            // Init now, populate later
            _characterUrlsFirstSeenInThisEpisode = new HashSet<string>();
        }

        public void AddCharacterUrlFirstSeenInThisEpisode(string characterUrl)
        {
            _characterUrlsFirstSeenInThisEpisode.Add(characterUrl);
        }

        public HashSet<string> GetCharacterUrlsFirstSeenInThisEpisode()
        {
            return _characterUrlsFirstSeenInThisEpisode;
        }
    }
}
