using System.Threading.Tasks;

namespace DataStorageManager.Repositories
{
    internal interface IDataRepository
    {
        Task<string> Read();
        Task Write(string data);
    }
}