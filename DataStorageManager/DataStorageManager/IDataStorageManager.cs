using System.Threading.Tasks;

namespace DataStorageManager
{
    public interface IDataStorageManager<DataType>
    {
        Task<DataType> Get();
        Task Save(DataType data);
    }
}