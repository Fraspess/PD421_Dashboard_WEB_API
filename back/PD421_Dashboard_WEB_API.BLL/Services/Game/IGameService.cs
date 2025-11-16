using PD421_Dashboard_WEB_API.BLL.Dtos.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.Services.Game
{
    public interface IGameService
    {
        Task<ServiceResponse> CreateGameAsync(CreateGameDto dto);
        Task<ServiceResponse> UpdateGameAsync(UpdateGameDto dto, string imagesPath);
        Task<ServiceResponse> DeleteGameAsync(string id);
        Task<ServiceResponse>? GetGameByIdAsync(string id);
        Task<ServiceResponse> GetAllGamesAsync();
        Task<ServiceResponse> GetGamesByGenreAsync(string genreName);
        Task<ServiceResponse> GetImageUrlByFileName(string fileName);
    }
}
