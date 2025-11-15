using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PD421_Dashboard_WEB_API.BLL.Services.BlobStorage;
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
        public async Task<string?> SaveImageAsync(IFormFile file, string storagePath)
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
                string imagePath = Path.Combine(storagePath, imageName);
                

                using (var stream = File.Create(imagePath)) // тимчасово зберігаємо файл локально
                {
                    await file.CopyToAsync(stream);
                }
                Console.WriteLine(imagePath);
                Console.WriteLine($"IMAGE NAME :: ${imageName}\nIMAGEPATH :: ${imagePath}");
                await _blobStorageService.UploadFileAsync(imageName, imageName, imagePath); // завантажуємо файл в блоб сховище
                File.Delete(imagePath); // видаляем файл після того як він завантажений в блоб сховище
                return imageName;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<string>> SaveImagesAsync(IEnumerable<IFormFile> files, string folderPath)
        {
            var tasks = files.Select(file => SaveImageAsync(file, folderPath));
            var results = await Task.WhenAll(tasks);
            return results.Where(res => res != null)!;
        }

        public async Task DeleteImageAsync(string containerName,string imagesPath)
        {
           await _blobStorageService.DeleteFileAsync(containerName,imagesPath);   
        }

        public async Task DeleteImageContainer(string containerName)
        {
            await _blobStorageService.DeleteBlobContainer(containerName);
        }
        
        public bool IsExists(string containerName, string filePath)
        {
           return _blobStorageService.IsFileExists(containerName,filePath);
        }


    }
}
