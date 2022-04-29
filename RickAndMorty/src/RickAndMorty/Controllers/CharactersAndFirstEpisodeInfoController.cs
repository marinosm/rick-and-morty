using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RickAndMortyEngine;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RickAndMorty
{
    [ApiController]
    [Route("[controller]")]
    public class CharactersAndFirstEpisodeInfoController : ControllerBase
    {
        private readonly ILogger<CharactersAndFirstEpisodeInfoController> _logger;
        private readonly ICharacterAndFirstEpisodeInfoEngine _engine;

        public CharactersAndFirstEpisodeInfoController(
            ILogger<CharactersAndFirstEpisodeInfoController> logger,
            ICharacterAndFirstEpisodeInfoEngine engine)
        {
            _logger = logger;
            _engine = engine;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery(Name = "name")] string name,
            [FromQuery(Name = "status")] string status,
            [FromQuery(Name = "species")] string species,
            [FromQuery(Name = "type")] string type,
            [FromQuery(Name = "gender")] string gender,
            [FromQuery(Name = "page")] int? page)
        {
            var requestId = Guid.NewGuid();
            try
            {
                var engineResponse = await _engine.FindCharactersAndFirstEpisodeInfo(new FindCharactersRequest
                {
                    Name = name,
                    Status = status,
                    Species = species,
                    Type = type,
                    Gender = gender,
                    Page = page
                });

                if (engineResponse.StatusCode == HttpStatusCode.OK)
                {
                    return new OkObjectResult(engineResponse.Data);
                }
                else
                {
                    return StatusCode((int)engineResponse.StatusCode, engineResponse.StatusMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(CharactersAndCoresidentsController));
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
