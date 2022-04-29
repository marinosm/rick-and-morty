using Microsoft.Extensions.Logging;
using RickAndMortyApiClient;
using RickAndMortyEngine;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RickAndMortyEngineDefault
{
    public class CharacterAndCoresidentsEngine : ICharacterAndCoresidentsEngine
    {
        private readonly ILogger<CharacterAndCoresidentsEngine> _logger;
        private readonly IClient _client;

        public CharacterAndCoresidentsEngine(ILogger<CharacterAndCoresidentsEngine> logger, IClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        public async Task<EngineResponse<Page<CharacterAndCoresidents>>> FindCharactersAndCoresidents(
            FindCharactersRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            // Init state
            var state = new CharacterAndCoresidentsState();

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
                return new EngineResponse<Page<CharacterAndCoresidents>>
                {
                    StatusCode = charactersClientResponse.ApiResponseCode,
                    StatusMessage = $"While fetching Characters: {charactersClientResponse.ApiResponseMessage}",
                };
            }

            var pageOfCharacters = charactersClientResponse.Data;
            var charactersPageInfo = pageOfCharacters.Info;
            var charactersInPage = pageOfCharacters.Results;

            // Collect Locations to load
            var missingLocationIds = state.InitializeCharacters(charactersInPage);

            // Load Locations from API
            var locationsClientResponse = await _client.GetMultipleLocations(missingLocationIds.ToList());
            if (locationsClientResponse.ApiResponseCode != HttpStatusCode.OK)
            {
                return new EngineResponse<Page<CharacterAndCoresidents>>
                {
                    StatusCode = charactersClientResponse.ApiResponseCode,
                    StatusMessage = $"While fetching Locations: {charactersClientResponse.ApiResponseMessage}",
                };
            }

            var locationDtos = locationsClientResponse.Data;

            // Update state
            state.InitializeLocations(locationDtos);

            // Prepare result
            return new EngineResponse<Page<CharacterAndCoresidents>>
            {
                StatusCode = HttpStatusCode.OK,
                StatusMessage = string.Empty,
                Data = new Page<CharacterAndCoresidents>
                {
                    TotalCount = charactersPageInfo.Count,
                    NumberOfPages = charactersPageInfo.Pages,
                    NextPageUrl = charactersPageInfo.Next,
                    PreviousPageUrl = charactersPageInfo.Prev,
                    Data = state.CharacterInfo
                }
            };
        }
    }
}
