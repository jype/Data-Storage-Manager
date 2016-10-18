using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;
using It = Machine.Specifications.It;
using Moq;
using MoqIt = Moq.It;
using DataStorageManager.StorageProviders;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Microsoft.Azure.KeyVault;

namespace DataStorageManager.Integration.Tests.StorageProviders
{
    [Subject("StorageProviders")]
    public class When_reading_and_writing_using_BlobStorageProvider
    {
        private static IStorageProvider blobStorageProvider;
        protected static string data;
        protected static string result;
        protected const string path = "tempdata/blob15263";

        Establish context = () =>
        {
            // Load certificate key
            X509KeyStorageFlags flags = X509KeyStorageFlags.Exportable;
            X509Certificate2 combinedCertificate = new X509Certificate2(
                AppDomain.CurrentDomain.BaseDirectory + @"\Certificate\privatekeyonlyfortest.pfx", "test", flags);
            var rsaKey = new RsaKey("privatetest", (RSACryptoServiceProvider)combinedCertificate.PrivateKey);

            blobStorageProvider = new BlobStorageProvider("UseDevelopmentStorage=true;", rsaKey);
            data = "test data";
        };

        Because of = () =>
        {
            blobStorageProvider.WriteString(path, data).Wait();
            result = blobStorageProvider.ReadString(path).Result;
        };

        It should_be_encrypted = () => blobStorageProvider.IsEncrypted.ShouldBeTrue();
        It should_match = () => result.ShouldBeLike(data);

        Cleanup after = () => blobStorageProvider.Delete(path).Wait();
    }
}
