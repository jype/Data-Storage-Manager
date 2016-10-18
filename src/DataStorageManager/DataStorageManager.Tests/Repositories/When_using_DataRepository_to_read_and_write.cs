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
    public class When_using_DataRepository_to_read_and_write
    {
        private static Mock<IStorageProvider> storageProviderMock;
        private static IDataRepository dataRepository;
        protected const string path = "testpath/test123";
        protected const string data = "test 123";
        protected static string result;

        Establish context = () =>
        {
            storageProviderMock = new Mock<IStorageProvider>();
            dataRepository = new DataRepository(storageProviderMock.Object, path, 600);
        };

        Because of = () =>
        {
            dataRepository.Write(data).Wait();
            result = dataRepository.Read().Result;
        };

        It should_match = () => result.ShouldBeLike(data);
        It should_never_use_StorageProvider_ReadString = () => 
            storageProviderMock.Verify(s => s.ReadString(MoqIt.IsAny<string>()), Times.Never, "Cache didn't work.");
    }
}
