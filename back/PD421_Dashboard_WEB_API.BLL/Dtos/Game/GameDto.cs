using PD421_Dashboard_WEB_API.BLL.Dtos.Genre;
using PD421_Dashboard_WEB_API.BLL.Dtos.Image;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.Dtos.Game
{
    public class GameDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;
        public string? Publisher { get; set; }
        public string? Developer { get; set; }
        public GameImageDto? MainImage { get; set; }
        public List<GameImageDto> Images { get; set; } = [];
        public List<GenreDto> Genres { get; set; } = [];
        
    }
}
