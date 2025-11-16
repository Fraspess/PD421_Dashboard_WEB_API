using Microsoft.AspNetCore.Mvc;
using PD421_Dashboard_WEB_API.BLL.Dtos.Game;
using PD421_Dashboard_WEB_API.BLL.Services;
using PD421_Dashboard_WEB_API.BLL.Services.Game;
using PD421_Dashboard_WEB_API.BLL.Settings;
using PD421_Dashboard_WEB_API.Extensions;
using System.Net;

namespace PD421_Dashboard_WEB_API.Controllers
{
    [ApiController]
    [Route("api/game")]
    public class GameController : Controller
    {
        private readonly IGameService _gameService;
        private readonly IWebHostEnvironment _environment;
        public GameController(IGameService gameService, IWebHostEnvironment environment)
        {
            _gameService = gameService;
            _environment = environment;
        }
        [HttpPost]
        public async Task<IActionResult> CreateGameAsync([FromForm] CreateGameDto dto)
        {
            dto.ReleaseDate = dto.ReleaseDate.ToUniversalTime();
            var response = await _gameService.CreateGameAsync(dto);
            return this.ToActionResult(response);

        }
        [HttpPut]
        public async Task<IActionResult> UpdateGameAsync([FromForm] UpdateGameDto dto)
        {

            //var response = await _gameService.UpdateGameAsync(dto);
            //return this.ToActionResult(response);
            var response = new ServiceResponse { Message = "D", IsSuccess = false, HttpStatusCode = HttpStatusCode.NotFound };
            return this.ToActionResult(response);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteGameAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                var validResponse = new ServiceResponse
                {
                    Message = "Id не вказано",
                    IsSuccess = false,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
                return this.ToActionResult(validResponse);
            }

            var response = await _gameService.DeleteGameAsync(id);
            return this.ToActionResult(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetGameByIdAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                var validResponse = new ServiceResponse
                {
                    Message = "Id не вказано",
                    IsSuccess = false,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
                return this.ToActionResult(validResponse);
            }
            var response = await _gameService.GetGameByIdAsync(id)!;
            return this.ToActionResult(response);
        }

        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllGamesAsync()
        {
            var response = await _gameService.GetAllGamesAsync();
            return this.ToActionResult(response);
        }

        [HttpGet]
        [Route("byGenre")]
        public async Task<IActionResult> GetGameByGenre(string? genreName)
        {
            if (String.IsNullOrEmpty(genreName))
            {
                var validResponse = new ServiceResponse
                {
                    Message = "Ім'я жанру не вказано",
                    IsSuccess = false,
                    HttpStatusCode = HttpStatusCode.BadRequest
                };
                return this.ToActionResult(validResponse);
            }
            var response = await _gameService.GetGamesByGenreAsync(genreName);
            return this.ToActionResult(response);
        }

        [HttpGet]
        [Route("image/ByFileName")]
        public async Task<IActionResult> GetImageUrlByFileName(string? fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                var responseV = new ServiceResponse { Message = "Назва картинки не вказана!", IsSuccess = false, HttpStatusCode = HttpStatusCode.BadRequest };
                return this.ToActionResult(responseV);
            }
            var response = await _gameService.GetImageUrlByFileName(fileName);
            return this.ToActionResult(response);
        }
    }
    }
