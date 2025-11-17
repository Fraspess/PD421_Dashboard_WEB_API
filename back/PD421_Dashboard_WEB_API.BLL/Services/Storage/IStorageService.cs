using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.Services.Storage
{
    public interface IStorageService
    {
        Task<string?> SaveImageAsync(IFormFile file, string gameId);
        Task<IEnumerable<string>> SaveImagesAsync(IEnumerable<IFormFile> files, string gameId);
        Task DeleteImageAsync(string imagePath);
    }
}
