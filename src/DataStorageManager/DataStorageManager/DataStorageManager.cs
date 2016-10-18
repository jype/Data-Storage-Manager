using DataStorageManager.Repositories;
using DataStorageManager.StorageProviders;
using Microsoft.Azure.KeyVault;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorageManager
{
    public class DataStorageManager<DataType> : IDataStorageManager<DataType>
    {
        private readonly IDataRepository dataRepository;

        private const double cacheTimeInSeconds = 600;

        internal DataStorageManager(IDataRepository dataRepository)
        {
            this.dataRepository = dataRepository;
        }

        public static DataStorageManager<T> CreateBlobDataStorage<T>(string connectionString, string path, RsaKey key =  null)
        {
            return new DataStorageManager<T>(new DataRepository(
                new BlobStorageProvider(connectionString, key), path, cacheTimeInSeconds));
        }

        public async Task<DataType> Get()
        {
            var jsonData = await this.dataRepository.Read();
            return JsonConvert.DeserializeObject<DataType>(jsonData);
        }

        public async Task Save(DataType data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            await this.dataRepository.Write(jsonData);
        }

    }
}
