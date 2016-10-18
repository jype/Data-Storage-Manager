using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorageManager.StorageProviders
{
    internal interface IStorageProvider
    {
        Task<string> ReadString(string path);
        Task WriteString(string path, string data);
        Task Delete(string path);
        bool IsEncrypted { get; }
    }
}
