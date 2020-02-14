using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace DocumentManagement.Persistence
{
    public class AzureBlobStorage
    {
        private readonly CloudBlobContainer _container;

        public AzureBlobStorage(string connectionString, string containerName)
        {
            _container = GetContainerReference(connectionString, containerName);
        }

        public static CloudBlobContainer GetContainerReference(string storageConnectionString, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            return blobClient.GetContainerReference(containerName);
        }

        public async Task<Uri> UploadFromStream(string blobName, Stream stream)
        {
            var blockBlob = _container.GetBlockBlobReference(blobName);
            await blockBlob.UploadFromStreamAsync(stream);

            return blockBlob.Uri;
        }

        public async Task Delete(string blobName)
        {
            var blockBlob = _container.GetBlockBlobReference(blobName);
            await blockBlob.DeleteIfExistsAsync();
        }
    }
}
