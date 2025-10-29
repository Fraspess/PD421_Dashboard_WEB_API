using PD421_Dashboard_WEB_API.DAL.Entitites;

namespace PD421_Dashboard_WEB_API.DAL.Repositories.Game
{
    public interface IGameRepository 
        : IGenericRepository<GameEntity, string>
    {
        Task<List<GenreEntity>> GetGenresByIdsAsync(List<string> genreIds);
        Task<List<GameEntity>> GetAllGamesAsync();
        Task<List<GameEntity>> GetGamesByGenreAsync(string genreName);
        new Task<GameEntity?> GetByIdAsync(string id);
    }
}
