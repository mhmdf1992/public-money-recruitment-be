using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MemoryStorage.Tests
{
    public class GetTests
    {
        protected IRepository<StorageItem> _repository;
        public GetTests()
        {
            _repository = new InMemoryRepository<StorageItem>();
        }
        [Fact]
        public void TestGetByKey_KeyExists_ReturnItem()
        {
            var item = new StorageItem()
            {
                Id = 1,
                Name = "test1"
            };

            _repository.Insert(item);
            var resultGet = _repository.Get(item.Key);
            Assert.Equal(item.Id, resultGet.Id);
            Assert.Equal(item.Name, resultGet.Name);
        }

        [Fact]
        public void TestGetByKey_KeyNotExists_ThrowsResourceNotFoundException()
        {
            Assert.Throws<Repository.Exceptions.ResourceNotFoundException>(() => _repository.Get("989785487484185"));
        }

        [Fact]
        public void TestAny_ConditionTrue_ReturnTrue()
        {
            var item = new StorageItem()
            {
                Id = 1,
                Name = "test1"
            };

            _repository.Insert(item);
            Assert.True(_repository.Any(x => x.Name == "test1"));
        }

        [Fact]
        public void TestAny_ConditionFalse_ReturnFalse()
        {
            Assert.False(_repository.Any(x => x.Name == "bananasss"));
        }

        [Fact]
        public void TestGetRange_ConditionTrue_ReturnRange()
        {
            var item = new StorageItem()
            {
                Id = 1,
                Name = "test1"
            };

            _repository.Insert(item);
            var result = _repository.Get(x => x.Name == "test1");
            Assert.Equal(item.Key, result.ElementAt(0).Key);
            Assert.Equal(item.Id, result.ElementAt(0).Id);
            Assert.Equal(item.Name, result.ElementAt(0).Name);
        }

        [Fact]
        public void TestGetRange_ConditionFalse_ReturnEmptyRange()
        {
            var result = _repository.Get(x => x.Name == "NAME554181510415");
            Assert.False(result.Any());
        }

        [Fact]
        public void TestCount_ReturnItemsCount()
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
            Assert.Equal(items.Count(), _repository.Count());
        }
    }
}