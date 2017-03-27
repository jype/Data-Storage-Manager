using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStorageManager.StorageProviders;

namespace DataStorageManager.Repositories
{
    internal class DataRepository : IDataRepository
    {
        private readonly IStorageProvider storageProvider;
        private readonly string path;
        private Dictionary<string, CacheItem> cacheList;
        private readonly double cacheTimeInSeconds;

        private class CacheItem
        {
            public DateTime timestamp { get; set; }
            public string data { get; set; }

            public bool IsValid(double cacheTimeInSeconds)
            {
                return (DateTime.UtcNow - timestamp).TotalSeconds < cacheTimeInSeconds;
            }
        }

        public DataRepository(IStorageProvider storageProvider, string path, double cacheTimeInSeconds = 600)
        {
            this.storageProvider = storageProvider;
            this.path = path;
            this.cacheTimeInSeconds = cacheTimeInSeconds;
            this.cacheList = new Dictionary<string, CacheItem>();
        }

        public async Task<string> Read()
        {
            CacheItem item;
            if (!cacheList.TryGetValue(this.path, out item) || !item.IsValid(this.cacheTimeInSeconds))
            {
                cacheList[this.path] = 
                    item = new CacheItem
                    {
                        timestamp = DateTime.UtcNow,
                        data = await this.storageProvider.ReadString(this.path).ConfigureAwait(false)
                    };
            }

            return item.data;
        }

        public async Task Write(string data)
        {
            await this.storageProvider.WriteString(this.path, data).ConfigureAwait(false);
            cacheList[this.path] = new CacheItem
            {
                timestamp = DateTime.UtcNow,
                data = data
            };
        }
    }
}
