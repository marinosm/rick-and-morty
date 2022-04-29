using Microsoft.Extensions.Logging;
using RickAndMortyApiClient;
using RickAndMortyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RickAndMortyEngineDefault
{
    public class CharacterAndFirstEpisodeInfoEngine : ICharacterAndFirstEpisodeInfoEngine
    {
        private readonly ILogger<CharacterAndFirstEpisodeInfoEngine> _logger;
        private readonly IClient _client;

        public CharacterAndFirstEpisodeInfoEngine(ILogger<CharacterAndFirstEpisodeInfoEngine> logger, IClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<EngineResponse<Page<CharacterAndFirstEpisodeInfo>>> FindCharactersAndFirstEpisodeInfo(
            FindCharactersRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            // Init state
            var state = new CharacterAndFirstEpisodeInfoState();

            // Load Characters from API
            var charactersClientResponse = await _client.FilterCharacters(new CharacterFilters
            {
                Name = request.Name,
                Status = request.Status,
                Species = request.Species,
                Type = request.Type,
                Gender = request.Gender,
                Page = request.Page
            });

            if (charactersClientResponse.ApiResponseCode != HttpStatusCode.OK)
            {
                return new EngineResponse<Page<CharacterAndFirstEpisodeInfo>>
                {
                    StatusCode = charactersClientResponse.ApiResponseCode,
                    StatusMessage = $"While fetching Characters: {charactersClientResponse.ApiResponseMessage}",
                };
            }

            var pageOfCharacters = charactersClientResponse.Data;
            var charactersPageInfo = pageOfCharacters.Info;
            var charactersInPage = pageOfCharacters.Results;

            // Collect Episodes to load
            var initialEpisodeIds = state.InitializeCharacters(charactersInPage);

            // Load Episodes from API
            var episodesClientResponse = await _client.GetMultipleEpisodes(initialEpisodeIds.ToList());
            if (episodesClientResponse.ApiResponseCode != HttpStatusCode.OK)
            {
                return new EngineResponse<Page<CharacterAndFirstEpisodeInfo>>
                {
                    StatusCode = charactersClientResponse.ApiResponseCode,
                    StatusMessage = $"While fetching Episodes: {charactersClientResponse.ApiResponseMessage}",
                };
            }

            var initialEpisodes = episodesClientResponse.Data;

            // Extract Episode Info from Episode Dtos
            state.InitializeEpisodesForCharacters(charactersInPage, initialEpisodes);

            // Collect characters not loaded so far
            var remainingCharacterIds = state.CollectMissingCharactersFromLoadedEpisodes();

            // Load remaining Characters from API
            var remainingCharactersClientResponse = await _client.GetMultipleCharacters(remainingCharacterIds.ToList());
            if (remainingCharactersClientResponse.ApiResponseCode != HttpStatusCode.OK)
            {
                return new EngineResponse<Page<CharacterAndFirstEpisodeInfo>>
                {
                    StatusCode = charactersClientResponse.ApiResponseCode,
                    StatusMessage = $"While fetching remaining Characters: {charactersClientResponse.ApiResponseMessage}",
                };
            }

            var remainingCharacterDtos = remainingCharactersClientResponse.Data;

            // Collect remaining Episodes to load
            var newDistictEpisodeIds = state.InitializeCharacters(remainingCharacterDtos);

            // Load remaining Episodes from API
            var remainingEpisodesClientResponse = await _client.GetMultipleEpisodes(newDistictEpisodeIds.ToList());
            if (remainingEpisodesClientResponse.ApiResponseCode != HttpStatusCode.OK)
            {
                return new EngineResponse<Page<CharacterAndFirstEpisodeInfo>>
                {
                    StatusCode = charactersClientResponse.ApiResponseCode,
                    StatusMessage = $"While fetching remaining Episodes: {charactersClientResponse.ApiResponseMessage}",
                };
            }

            var remainingEpisodeDtos = remainingEpisodesClientResponse.Data;

            // Extract Episode Info from Episode Dtos
            // and add to original collection
            state.InitializeEpisodesForCharacters(remainingCharacterDtos, remainingEpisodeDtos);

            // We have loaded everything and have set the earliest episode for every character
            // We can now populate characters first seen in each episode
            state.PopulateCharactersFirstSeenInEpisode();

            // Prepare result
            var results = new List<CharacterAndFirstEpisodeInfo>(charactersInPage.Count());
            foreach (var character in charactersInPage)
            {
                var characterInfo = state.CharacterInfoPerCharacterUrl[character.Url];
                var earliestEpisode = characterInfo.GetEarliestEpisodeUrl();
                var episodeInfo = state.EpisodeInfoPerEpisodeUrl[earliestEpisode];
                var firstSeenCharacters = episodeInfo.GetCharacterUrlsFirstSeenInThisEpisode();

                var item = new CharacterAndFirstEpisodeInfo
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
                    FirstSeenInEpisode = earliestEpisode,
                    OtherCharactersFirstSeenInTheEpisode = firstSeenCharacters.ToArray()
                };

                results.Add(item);
            }

            return new EngineResponse<Page<CharacterAndFirstEpisodeInfo>>
            {
                StatusCode = HttpStatusCode.OK,
                StatusMessage = string.Empty,
                Data = new Page<CharacterAndFirstEpisodeInfo>
                {
                    TotalCount = charactersPageInfo.Count,
                    NumberOfPages = charactersPageInfo.Pages,
                    NextPageUrl = charactersPageInfo.Next,
                    PreviousPageUrl = charactersPageInfo.Prev,
                    Data = results
                }
            };
        }

    }
}
