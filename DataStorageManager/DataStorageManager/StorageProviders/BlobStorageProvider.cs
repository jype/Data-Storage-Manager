using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Azure.KeyVault;
using System.Threading;
using System.IO;

namespace DataStorageManager.StorageProviders
{
    internal class BlobStorageProvider : IStorageProvider
    {
        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;
        private readonly BlobRequestOptions options;
        private readonly Encoding encoding;

        private class StorageClient : IDisposable
        {
            public CloudBlobContainer blobContainer;
            public CloudBlockBlob blockBlob;

            public StorageClient(CloudBlobClient blobClient, string path, bool createIfNotExists = false)
            {
                blobContainer = blobClient.GetContainerReference(this.GetContainerName(path));
                if (createIfNotExists)
                {
                    blobContainer.CreateIfNotExists(BlobContainerPublicAccessType.Off,
                        new BlobRequestOptions
                        {
                            RequireEncryption = true
                        },
                        null);
                }
                blockBlob = blobContainer.GetBlockBlobReference(this.GetBlobName(path));
            }

            private string GetContainerName(string path)
            {
                return path.Split('/')[0];
            }

            private string GetBlobName(string path)
            {
                return path.Substring(path.IndexOf('/') + 1);
            }

            public void Dispose()
            {
                // Do we have anything to dispose of?
                this.blockBlob = null;
                this.blobContainer = null;
            }
        };
        
        public BlobStorageProvider(string connectionString, RsaKey key = null)
        {
            this.storageAccount = CloudStorageAccount.Parse(connectionString);
            this.encoding = Encoding.UTF8;

            // Create the encryption policy to be used for upload and download.
            BlobEncryptionPolicy policy = new BlobEncryptionPolicy(key, null);

            this.options = new BlobRequestOptions
            {
                RequireEncryption = key != null,
                EncryptionPolicy = policy
            };

            blobClient = storageAccount.CreateCloudBlobClient();
        }

        public bool IsEncrypted { get { return this.options.RequireEncryption.HasValue && this.options.RequireEncryption.Value; } }

        public async Task<string> ReadString(string path)
        {
            using (var client = new StorageClient(this.blobClient, path))
            {
                return await client.blockBlob.DownloadTextAsync(this.encoding, null, this.options, null).ConfigureAwait(false);
            }
        }

        public async Task WriteString(string path, string data)
        {
            using (var client = new StorageClient(this.blobClient, path, true))
            {
                var buffer = this.encoding.GetBytes(data);
                await client.blockBlob.UploadFromByteArrayAsync(buffer, 0, buffer.Count(), null, this.options, null).ConfigureAwait(false);
            }
        }

        public async Task Delete(string path)
        {
            using (var client = new StorageClient(this.blobClient, path))
            {
                await client.blockBlob.DeleteAsync().ConfigureAwait(false);
            }
        }
    }
}
