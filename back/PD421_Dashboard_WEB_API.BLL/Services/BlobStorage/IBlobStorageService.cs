using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.Services.BlobStorage
{
    public interface IBlobStorageService
    {
        public Task<BlobContainerClient> GetContainerClientAsync(string containerName);
        public Task UploadFileAsync(string containerName, string destFilePath, IFormFile file);
        public Task DownloadFileAsync(string containerName, string destFile, string srcFilePath);
        public Task DeleteFileAsync(string containerName, string filePath);
        public bool IsContainerExists(string containerName);
        public Task DeleteBlobContainer(string containerName);
        public bool IsFileExists(string containerName, string filePath);
        public Task<string> GetUrlByFileName(string containerName, string filePath);

    }
}
