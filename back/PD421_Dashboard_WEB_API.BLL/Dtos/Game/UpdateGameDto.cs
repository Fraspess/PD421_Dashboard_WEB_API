using Microsoft.AspNetCore.Http;
using PD421_Dashboard_WEB_API.BLL.Dtos.Image;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.Dtos.Game
{
    public class UpdateGameDto
    {
        public required string Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? Publisher { get; set; }
        public string? Developer { get; set; }
        public List<string>? GenreIds { get; set; }
        public IFormFile? MainImage { get; set; }
        public List<IFormFile> Images { get; set; } = [];
    }
}
