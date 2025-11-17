using Microsoft.AspNetCore.Http;
using PD421_Dashboard_WEB_API.BLL.Services.BlobStorage;
using PD421_Dashboard_WEB_API.BLL.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.Services.Storage
{
    public class StorageService : IStorageService
    {
        private readonly IBlobStorageService _blobStorageService;
        public StorageService(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }
        public async Task<string?> SaveImageAsync(IFormFile file, string gameId)
        {
            try
            {
                var types = file.ContentType.Split('/');
                if (types.Length != 2 || types[0] != "image")
                {
                    return null;
                }

                string extension = Path.GetExtension(file.FileName);
                string imageName = $"{Guid.NewGuid()}{extension}";
                string imagePath = Path.Combine(gameId, imageName);
                await _blobStorageService.UploadFileAsync(StaticFilesSettings.ImagesContainerName,imagePath,file);
                return imageName;
            
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<string>> SaveImagesAsync(IEnumerable<IFormFile> files, string gameId)
        {
            var tasks = files.Select(file => SaveImageAsync(file, gameId));
            var results = await Task.WhenAll(tasks);
            return results.Where(res => res != null)!;
        }

        public async Task DeleteImageAsync(string imagePath)
        {
            await _blobStorageService.DeleteFileAsync(StaticFilesSettings.ImagesContainerName,imagePath);
        }


    }
}
