using Repository;
using System;
using System.Linq;
using Xunit;

namespace MemoryStorage.Tests
{
    public class DeleteTests
    {
        protected IRepository<StorageItem> _repository;
        public DeleteTests()
        {
            _repository = new InMemoryRepository<StorageItem>();
        }
        [Fact]
        public void TestDelete_GetByKeyDeleted_ThrowResourceNotFoundException()
        {
            var item = new StorageItem()
            {
                Id = 1,
                Name = "test1"
            };

            _repository.Insert(item);
            var resultKey = _repository.Delete(item.Key);
            
            Assert.Equal(item.Key, resultKey);
            Assert.Throws<Repository.Exceptions.ResourceNotFoundException>(() => _repository.Get(item.Key));
        }

        [Fact]
        public void TestDeleteRange_GetByKeyAnyDeleted_ThrowResourceNotFoundException()
        {
            var items = new StorageItem[]
            {
                new StorageItem()
                {
                    Id = 1,
                    Name = "test1"
                },new StorageItem()
                {
                    Id = 2,
                    Name = "test2"
                },
            };

            _repository.InsertRange(items);
            _repository.DeleteRange(items.Select(x => x.Key));
            Assert.Throws<Repository.Exceptions.ResourceNotFoundException>(() => _repository.Get(new Random().Next(1,2).ToString()));
        }

        [Fact]
        public void TestDeleteRangeDuplicateKeys_ThrowsUniqueKeyContraintException()
        {
            var items = new StorageItem[]
            {
                new StorageItem()
                {
                    Id = 10,
                    Name = "test10"
                },new StorageItem()
                {
                    Id = 10,
                    Name = "test20"
                },
            };

            Assert.Throws<Repository.Exceptions.UniqueKeyContraintException>(() => _repository.DeleteRange(items.Select(x => x.Key)));
        }

        [Fact]
        public void TestClear_Any_ReturnFalse()
        {
            var items = new StorageItem[]
            {
                new StorageItem()
                {
                    Id = 100,
                    Name = "test100"
                },new StorageItem()
                {
                    Id = 101,
                    Name = "test101"
                },
            };
            _repository.InsertRange(items);
            _repository.Clear();
            Assert.False(_repository.Any());
        }
    }
}