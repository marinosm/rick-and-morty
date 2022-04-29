using RickAndMortyApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RickAndMortyEngineDefault
{
    internal class CharacterAndFirstEpisodeInfoState
    {
        private readonly Regex _episodeIdRegex = new Regex("episode/([0-9]+)");
        private readonly Regex _characterIdRegex = new Regex("character/([0-9]+)");

        internal Dictionary<string, CharacterInfo> CharacterInfoPerCharacterUrl = new Dictionary<string, CharacterInfo>();
        internal Dictionary<string, EpisodeInfo> EpisodeInfoPerEpisodeUrl = new Dictionary<string, EpisodeInfo>();

        /// <summary>
        /// Updates Characters state in-place.
        /// Earliest Episode per Character is NOT set.
        /// </summary>
        /// <param name="characterDtos">Newly loaded Characters.</param>
        /// <returns>Episode Ids not loaded so far.</returns>
        internal HashSet<string> InitializeCharacters(IEnumerable<CharacterDto> characterDtos)
        {
            if (characterDtos is null) throw new ArgumentNullException(nameof(characterDtos));

            var missingEpisodeIds = new HashSet<string>();

            foreach (var character in characterDtos)
            {
                CharacterInfoPerCharacterUrl[character.Url] = new CharacterInfo(character);

                // Extract Episode Ids from Episode Urls
                foreach (var episodeUrl in character.Episode)
                {
                    if (!EpisodeInfoPerEpisodeUrl.ContainsKey(episodeUrl))
                    {
                        var match = _episodeIdRegex.Match(episodeUrl);
                        if (match.Groups.Count == 2)
                        {
                            var episodeId = match.Groups[1].Value;
                            missingEpisodeIds.Add(episodeId);
                        }
                    }
                }
            }

            return missingEpisodeIds;
        }

        /// <summary>
        /// Updates Episode state in place.
        /// Updates Character earliest episode state in-place.
        /// </summary>
        /// <param name="characterDtos">Characters to update. If null, all Characters in state will be used.</param>
        /// <param name="episodeDtos">Newly loaded episodes.</param>
        internal void InitializeEpisodesForCharacters(IEnumerable<CharacterDto> characterDtos, List<EpisodeDto> episodeDtos)
        {
            if (episodeDtos is null) throw new ArgumentNullException(nameof(episodeDtos));
            // Null characterDtos is allowed

            foreach (var episode in episodeDtos)
            {
                EpisodeInfoPerEpisodeUrl[episode.Url] = new EpisodeInfo(episode);
            }

            // Choose whether to iterate over given Characters or all loaded Characters
            var characters = characterDtos == null ?
                CharacterInfoPerCharacterUrl.Values.Select(c => c.Dto) :
                characterDtos;

            // Find and set earliest Episode per Character
            foreach (var character in characters)
            {
                var minAirDate = DateTime.MaxValue;
                var minEpisodeUrl = string.Empty;
                foreach (var episodeUrl in character.Episode)
                {
                    var episode = EpisodeInfoPerEpisodeUrl[episodeUrl];

                    if (episode.AirDate <= minAirDate)
                    {
                        minAirDate = episode.AirDate;
                        minEpisodeUrl = episodeUrl;
                    }
                }

                CharacterInfoPerCharacterUrl[character.Url].SetEarliestEpisodeUrl(minEpisodeUrl);
            }
        }

        /// <summary>
        /// Iterates through internal state.
        /// No state updates take place.
        /// </summary>
        /// <returns>Distinct Character Ids not loaded so far.</returns>
        internal HashSet<string> CollectMissingCharactersFromLoadedEpisodes()
        {
            var remainingCharacterIds = new HashSet<string>();

            foreach (var (episodeUrl, episodeInfo) in EpisodeInfoPerEpisodeUrl)
            {
                foreach (var characterUrl in episodeInfo.CastUrls)
                {
                    if (!CharacterInfoPerCharacterUrl.ContainsKey(characterUrl))
                    {
                        // Extract Character Ids from Character Urls
                        var match = _characterIdRegex.Match(characterUrl);
                        if (match.Groups.Count == 2)
                        {
                            var characterId = match.Groups[1].Value;
                            remainingCharacterIds.Add(characterId);
                        }
                    }
                }
            }

            return remainingCharacterIds;
        }

        /// <summary>
        /// Assuming "earliest episode per character" is already set.
        /// Updates Characters "first seen in each episode" state in-place.
        /// </summary>
        internal void PopulateCharactersFirstSeenInEpisode()
        {
            foreach (var (characterUrl, characterInfo) in CharacterInfoPerCharacterUrl)
            {
                var earliestEpisode = characterInfo.GetEarliestEpisodeUrl();
                var episodeInfo = EpisodeInfoPerEpisodeUrl[earliestEpisode];

                episodeInfo.AddCharacterUrlFirstSeenInThisEpisode(characterUrl);
            }
        }
    }
}
