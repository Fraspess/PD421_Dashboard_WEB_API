using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.Dtos.Game
{
    public class CreateGameDto
    {
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public double Price { get; set; }
        public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;
        public string? Publisher { get; set; }
        public string? Developer { get; set; }
        public required List<string> GenreIds { get; set; }

        [Required(ErrorMessage ="Картинка є обов'язковою")]
        public required IFormFile MainImage { get; set; }
        public List<IFormFile> Images { get; set; } = [];
    }
}
