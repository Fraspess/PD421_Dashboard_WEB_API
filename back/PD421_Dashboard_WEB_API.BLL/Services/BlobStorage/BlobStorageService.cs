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

        public async Task DownloadFileAsync(string containerName, string destFilePath, string srcFilePath)
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blob = containerClient.GetBlobClient(srcFilePath);

            await blob.DownloadToAsync(destFilePath);
        }

        public async Task DeleteFileAsync(string containerName, string filePath)
        {
            var containerClient = await GetContainerClientAsync(containerName);
            var blob = containerClient.GetBlobClient(filePath);
            await blob.DeleteIfExistsAsync();
        }

        public bool IsContainerExists(string containerName)
        {
            var containerClient = _serviceClient.GetBlobContainerClient(containerName);
            Response<bool> existsResponse = containerClient.Exists();
            return existsResponse.Value;
        }

        public bool IsFileExists(string containerName, string filePath)
        {
            var containerClient = _serviceClient.GetBlobContainerClient(containerName);
            var blob = containerClient.GetBlobClient(filePath);
            Response<bool> existsResponse = blob.Exists();
            return existsResponse.Value;
        }

        public async Task DeleteBlobContainer(string containerName)
        {
            try
            {
                var containerClient = _serviceClient.GetBlobContainerClient(containerName);
                await containerClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<string> GetUrlByFileName(string containerName, string filePath)
        {
            var containerClient = _serviceClient.GetBlobContainerClient(containerName);
            var blob = containerClient.GetBlobClient(filePath);
            if(await blob.ExistsAsync())
            {
                return blob.Uri.ToString();
            }
            return "";
        }
    }
}
