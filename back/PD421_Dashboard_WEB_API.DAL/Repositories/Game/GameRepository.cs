using Microsoft.EntityFrameworkCore;
using PD421_Dashboard_WEB_API.DAL.Entitites;

namespace PD421_Dashboard_WEB_API.DAL.Repositories.Game
{
    public class GameRepository 
        : GenericRepository<GameEntity, string>, IGameRepository
    {
        public GameRepository(AppDbContext context)
            : base(context) { }

        public async Task<List<GameEntity>> GetAllGamesAsync()
        {
            var games = await _context.Games
                .Include(g => g.Genres)
                .Include(i => i.Images)
                .ToListAsync();
            return games;
        }

        public async Task<List<GameEntity>> GetGamesByGenreAsync(string genreName)
        {
            var games = await _context.Games
                .Include(g => g.Genres)
                .Where(g => g.Genres.Any(genre => genre.Name == genreName))
                .ToListAsync();
            return games;
        }

        public async Task<List<GenreEntity>> GetGenresByIdsAsync(List<string> genreIds)
        {
            List<GenreEntity> genres = await _context.Genres
                .Where(g => genreIds.Contains(g.Id))
                .ToListAsync();
            return genres;
        }
        public new async Task<GameEntity?> GetByIdAsync(string id)
        {
            return await _context.Games
                .Include(g => g.Genres)
                .Include(i => i.Images)
                .FirstOrDefaultAsync(g => g.Id == id);
        }
    }
}
