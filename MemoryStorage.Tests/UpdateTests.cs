using Repository;
using System.Linq;
using Xunit;

namespace MemoryStorage.Tests
{
    public class UpdateTests
    {
        protected IRepository<StorageItem> _repository;
        public UpdateTests()
        {
            _repository = new InMemoryRepository<StorageItem>();
        }

        [Fact]
        public void TestUpdate_GetById_ReturnUpdated()
        {
            var item = new StorageItem()
            {
                Id = 1,
                Name = "test1"
            };

            var updateItem = new StorageItem()
            {
                Id = 1,
                Name = "testUpdate"
            };

            _repository.Insert(item);
            var resultKey = _repository.Update(updateItem.Key, oldItem => oldItem.Name = updateItem.Name);
            var resultGet = _repository.Get(resultKey);

            Assert.Equal(updateItem.Key, resultKey);
            Assert.Equal(updateItem.Id, resultGet.Id);
            Assert.Equal(updateItem.Name, resultGet.Name);
        }
    }
}