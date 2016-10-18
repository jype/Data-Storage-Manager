using Machine.Specifications;
using It = Machine.Specifications.It;
using Moq;
using MoqIt = Moq.It;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStorageManager;
using DataStorageManager.Repositories;

namespace DataStorageManager.Tests
{
    public class When_using_DataStorageManager_to_load_and_save
    {
        private static Mock<IDataRepository> dataRepositoryMock;
        protected static IDataStorageManager<Dictionary<int, string>> dataStorageManager;
        protected static Dictionary<int, string> data;
        protected static Dictionary<int, string> resultData;
        protected const string jsonData = "{\"1\":\"abc\",\"2\":\"def\"}";
        protected static string result;

        private Establish context = () =>
        {
            data = new Dictionary<int, string>
            {
                {1, "abc"},
                {2, "def"}
            };
            dataRepositoryMock = new Mock<IDataRepository>();
            dataRepositoryMock.Setup(d => d.Write(MoqIt.IsAny<string>())).Callback<string>(
                (c) => result = c).Returns((Task)Task.FromResult(false));
            dataRepositoryMock.Setup(d => d.Read()).Returns(Task.FromResult(jsonData));
            dataStorageManager = new DataStorageManager<Dictionary<int, string>>(dataRepositoryMock.Object);
        };

        private Because of = () =>
        {
            dataStorageManager.Save(data).Wait();
            resultData = dataStorageManager.Get().Result;
        };

        It should_write_serialized = () => result.ShouldBeLike(jsonData);
        It should_return_data = () => resultData.ShouldBeLike(data);
    }
}
