using RickAndMortyApiClient;
using RickAndMortyEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RickAndMortyEngineDefault
{
    internal class CharacterAndCoresidentsState
    {
        private readonly Regex _locationIdRegex = new Regex("location/([0-9]+)");

        internal readonly List<CharacterAndCoresidents> CharacterInfo = new List<CharacterAndCoresidents>();
        internal Dictionary<string, LocationInfo> LocationInfoPerLocationUrl = new Dictionary<string, LocationInfo>();

        private readonly IDictionary<string, IList<int>> _characterIdxPerLocationUrl = new Dictionary<string, IList<int>>();

        /// <summary>
        /// Updates Characters state in-place.
        /// </summary>
        /// <param name="characterDtos">Newly loaded Characters.</param>
        /// <returns>Location Ids not loaded so far.</returns>
        internal HashSet<string> InitializeCharacters(IList<CharacterDto> characterDtos)
        {
            var distinctLocationIdsNotFound = new HashSet<string>();

            for (int i = 0; i < characterDtos.Count; i++)
            {
                var character = characterDtos[i];
                var characterAndCoresidents = new CharacterAndCoresidents
                {
                    Id = character.Id,
                    Name = character.Name,
                    Status = character.Status,
                    Species = character.Species,
                    Type = character.Type,
                    Gender = character.Gender,
                    Origin = new CharacterLocation { Name = character.Origin?.Name, Url = character.Origin?.Url },
                    Location = new CharacterLocation { Name = character.Location?.Name, Url = character.Location?.Url },
                    Image = character.Image,
                    Episode = character.Episode,
                    Url = character.Url,
                    Created = character.Created,
                    OtherCharactersInLocation = null
                };

                CharacterInfo.Add(characterAndCoresidents);

                var locationUrl = character.Location?.Url;

                if (locationUrl != null)
                {
                    if (LocationInfoPerLocationUrl.TryGetValue((locationUrl), out var locationDto))
                    {
                        characterAndCoresidents.OtherCharactersInLocation = locationDto.Residents;
                    }
                    else
                    {
                        // Add this Character as pending location info
                        if (_characterIdxPerLocationUrl.TryGetValue(locationUrl, out var characterIdxs))
                        {
                            characterIdxs.Add(i);
                        }
                        else
                        {
                            _characterIdxPerLocationUrl[locationUrl] = new List<int> { i };
                        }

                        // Extract Location Id from Location Url
                        var match = _locationIdRegex.Match(locationUrl);
                        if (match.Groups.Count == 2)
                        {
                            var locationId = match.Groups[1].Value;
                            distinctLocationIdsNotFound.Add(locationId);
                        }
                    }
                }
            }

            return distinctLocationIdsNotFound;
        }

        /// <summary>
        /// Updates Locations state in-place.
        /// Updates Character "other residents" state in-place.
        /// </summary>
        /// <param name="locationDtos">Newly loaded Locations.</param>
        internal void InitializeLocations(IEnumerable<LocationDto> locationDtos)
        {
            foreach (var location in locationDtos)
            {
                // Update Locations for future Characters to find
                LocationInfoPerLocationUrl[location.Url] = new LocationInfo(location);

                // Update any Characters already loaded
                var characterIdxs = _characterIdxPerLocationUrl[location.Url];
                foreach (var idx in characterIdxs)
                {
                    CharacterInfo[idx].OtherCharactersInLocation = location.Residents;
                }
            }
        }
    }
}
