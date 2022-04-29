using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RickAndMortyApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyApiClientDefault
{
    public class Client : IClient
    {
        private const string CHARACTERS_RELATIVE_PATH = "character";
        private const string LOCATIONS_RELATIVE_PATH = "location";
        private const string EPISODES_RELATIVE_PATH = "episode";

        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        /// <param name="logger"></param>
        /// <param name="httpClientFactory">Base address must be set</param>
        public Client(ILogger<Client> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("default");

            if (_httpClient.BaseAddress == null)
            {
                throw new ArgumentException($"httpClient.BaseAddress must be set");
            }
        }

        public async Task<ClientResponse<PageDto<CharacterDto>>> FilterCharacters(CharacterFilters filters)
        {
            if (filters == null) throw new ArgumentNullException(nameof(filters));

            var builder = new StringBuilder();
            builder.Append(CHARACTERS_RELATIVE_PATH);

            var paramsBuilder = filters.ToQueryStringBuilder();
            if (paramsBuilder.Length > 0)
            {
                builder.Append('?');
                builder.Append(paramsBuilder);
            }

            var relativePath = builder.ToString();
            var uri = new Uri(relativePath, UriKind.Relative);

            var apiResponse = await _httpClient.GetAsync(uri);

            var clientResponse = new ClientResponse<PageDto<CharacterDto>>
            {
                ApiResponseCode = apiResponse.StatusCode,
                ApiResponseMessage = apiResponse.ReasonPhrase
            };

            if (apiResponse.IsSuccessStatusCode)
            {
                if (apiResponse.Content is null) throw new Exception("Content was null");

                var apiResponseContent = await apiResponse.Content.ReadAsStringAsync();
                var pageDto = JsonConvert.DeserializeObject<PageDto<CharacterDto>>(apiResponseContent);

                clientResponse.Data = pageDto;

            }

            return clientResponse;
        }

        /// <summary>
        /// This Client implementation returns an empty list of Dtos when an empty list of Ids is passed
        /// </summary>
        public async Task<ClientResponse<List<CharacterDto>>> GetMultipleCharacters(List<string> ids)
        {
            if (ids is null) throw new ArgumentNullException(nameof(ids));

            if (ids.Count == 0)
            {
                return new ClientResponse<List<CharacterDto>>
                {
                    ApiResponseCode = HttpStatusCode.OK,
                    ApiResponseMessage = string.Empty,
                    Data = new List<CharacterDto>(0)
                };
            }

            var builder = new StringBuilder(CHARACTERS_RELATIVE_PATH);
            builder.Append('/');
            ids.Aggregate(builder, (builder, i) => builder.Append(i).Append(','));
            builder.Remove(builder.Length - 1, 1);

            var uri = builder.ToString();

            var apiResponse = await _httpClient.GetAsync(uri);

            var clientResponse = new ClientResponse<List<CharacterDto>>
            {
                ApiResponseCode = apiResponse.StatusCode,
                ApiResponseMessage = apiResponse.ReasonPhrase
            };

            if (apiResponse.IsSuccessStatusCode)
            {
                if (apiResponse.Content is null) throw new Exception("Content was null.");

                var apiResponseContent = await apiResponse.Content.ReadAsStringAsync();
                List<CharacterDto> characterDtos;

                if (ids.Count == 1)
                {
                    var characterDto = JsonConvert.DeserializeObject<CharacterDto>(apiResponseContent);
                    characterDtos = new List<CharacterDto> { characterDto };
                }
                else if (ids.Count > 0)
                {
                    characterDtos = JsonConvert.DeserializeObject<List<CharacterDto>>(apiResponseContent);
                }
                else
                {
                    throw new NotImplementedException($"Getting all characters via {nameof(GetMultipleCharacters)} is currently not supported.");
                }
                clientResponse.Data = characterDtos;
            }

            return clientResponse;
        }

        /// <summary>
        /// This Client implementation returns an empty list of Dtos when an empty list of Ids is passed
        /// </summary>
        public async Task<ClientResponse<List<EpisodeDto>>> GetMultipleEpisodes(List<string> ids)
        {
            if (ids is null) throw new ArgumentNullException(nameof(ids));

            if (ids.Count == 0)
            {
                return new ClientResponse<List<EpisodeDto>>
                {
                    ApiResponseCode = HttpStatusCode.OK,
                    ApiResponseMessage = string.Empty,
                    Data = new List<EpisodeDto>(0)
                };
            }

            var builder = new StringBuilder(EPISODES_RELATIVE_PATH);
            builder.Append('/');
            ids.Aggregate(builder, (builder, i) => builder.Append(i).Append(','));
            builder.Remove(builder.Length - 1, 1);

            var uri = builder.ToString();

            var apiResponse = await _httpClient.GetAsync(uri);

            var clientResponse = new ClientResponse<List<EpisodeDto>>
            {
                ApiResponseCode = apiResponse.StatusCode,
                ApiResponseMessage = apiResponse.ReasonPhrase
            };

            if (apiResponse.IsSuccessStatusCode)
            {
                if (apiResponse.Content is null) throw new Exception("Content was null.");

                var apiResponseContent = await apiResponse.Content.ReadAsStringAsync();
                List<EpisodeDto> episodeDtos;
                if (ids.Count == 1)
                {
                    var episodeDto = JsonConvert.DeserializeObject<EpisodeDto>(apiResponseContent);
                    episodeDtos = new List<EpisodeDto> { episodeDto };
                }
                else if (ids.Count > 0)
                {
                    episodeDtos = JsonConvert.DeserializeObject<List<EpisodeDto>>(apiResponseContent);
                }
                else
                {
                    throw new NotImplementedException($"Getting all episodes via {nameof(GetMultipleEpisodes)} is currently not supported.");
                }
                clientResponse.Data = episodeDtos;
            }

            return clientResponse;
        }

        /// <summary>
        /// This Client implementation returns an empty list of Dtos when an empty list of Ids is passed
        /// </summary>
        public async Task<ClientResponse<List<LocationDto>>> GetMultipleLocations(List<string> ids)
        {
            if (ids is null) throw new ArgumentNullException(nameof(ids));

            if (ids.Count == 0)
            {
                return new ClientResponse<List<LocationDto>>
                {
                    ApiResponseCode = HttpStatusCode.OK,
                    ApiResponseMessage = string.Empty,
                    Data = new List<LocationDto>(0)
                };
            }

            var builder = new StringBuilder(LOCATIONS_RELATIVE_PATH);
            builder.Append('/');
            ids.Aggregate(builder, (builder, i) => builder.Append(i).Append(','));
            builder.Remove(builder.Length - 1, 1);

            var uri = builder.ToString();

            var apiResponse = await _httpClient.GetAsync(uri);

            var clientResponse = new ClientResponse<List<LocationDto>>
            {
                ApiResponseCode = apiResponse.StatusCode,
                ApiResponseMessage = apiResponse.ReasonPhrase
            };

            if (apiResponse.IsSuccessStatusCode)
            {
                if (apiResponse.Content is null) throw new Exception("Content was null.");

                var apiResponseContent = await apiResponse.Content.ReadAsStringAsync();
                List<LocationDto> locationDtos;
                if (ids.Count == 1)
                {
                    var locationDto = JsonConvert.DeserializeObject<LocationDto>(apiResponseContent);
                    locationDtos = new List<LocationDto> { locationDto };
                }
                else if (ids.Count > 0)
                {
                    locationDtos = JsonConvert.DeserializeObject<List<LocationDto>>(apiResponseContent);
                }
                else
                {
                    throw new NotImplementedException($"Getting all locations via {nameof(GetMultipleLocations)} is currently not supported.");
                }
                clientResponse.Data = locationDtos;
            }

            return clientResponse;
        }
    }
}
