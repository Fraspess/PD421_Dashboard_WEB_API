using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.Services.BlobStorage
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _serviceClient;

        public BlobStorageService(string connectionString)
        {
            _serviceClient = new BlobServiceClient(connectionString);
        }

        public async Task<BlobContainerClient> GetContainerClientAsync(string containerName)
        {
            var containerClient = _serviceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);
            return containerClient;
        }

        public async Task UploadFileAsync(string containerName, string destFilePath, IFormFile file)
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blob = containerClient.GetBlobClient(destFilePath);

            using var fileStream = file.OpenReadStream();
            await blob.UploadAsync(fileStream, overwrite: true);
        }

        public async Task DeleteFileAsync(string containerName, string filePath)
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blob = containerClient.GetBlobClient(filePath);
            await blob.DeleteIfExistsAsync();
        }

    }
}
