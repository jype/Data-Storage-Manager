using Machine.Specifications;
using It = Machine.Specifications.It;
using Moq;
using MoqIt = Moq.It;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStorageManager.StorageProviders;
using DataStorageManager.Repositories;

namespace DataStorageManager.Tests.Repositories
{
    [Subject("Repositories")]
    public class When_using_DataRepository_to_read_and_write_and_cache_timeouts
    {
        private static Mock<IStorageProvider> storageProviderMock;
        private static IDataRepository dataRepository;
        protected const string path = "testpath/test123";
        protected const string data = "test 123";
        protected static string result;

        Establish context = () =>
        {
            storageProviderMock = new Mock<IStorageProvider>();
            storageProviderMock.Setup(s => s.ReadString(path)).Returns(Task.FromResult<string>(data));
            dataRepository = new DataRepository(storageProviderMock.Object, path, 0);
        };

        Because of = () =>
        {
            dataRepository.Write(data).Wait();
            Task.Delay(100).Wait();
            result = dataRepository.Read().Result;
        };

        It should_match = () => result.ShouldBeLike(data);
        It should_only_use_StorageProvider_ReadString_once = () => 
            storageProviderMock.Verify(s => s.ReadString(MoqIt.IsAny<string>()), Times.Once, "Cache ondemand didn't work.");
    }
}
