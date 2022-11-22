using Repository;
using System.Linq;
using Xunit;

namespace MemoryStorage.Tests
{
    public class InsertTests
    {
        protected IRepository<StorageItem> _repository;
        public InsertTests()
        {
            _repository = new InMemoryRepository<StorageItem>();
        }
        [Fact]
        public void TestInsert_GetById_ReturnInserted()
        {
            var item = new StorageItem()
            {
                Id = 1,
                Name = "test1"
            };

            var resultKey = _repository.Insert(item);

            var resultGet = _repository.Get(resultKey);
            
            Assert.Equal(item.Key, resultKey);
            Assert.Equal(item.Id, resultGet.Id);
            Assert.Equal(item.Name, resultGet.Name);
        }

        [Fact]
        public void TestInsert_KeyExists_ThrowsUniqueKeyContraintException()
        {
            var item = new StorageItem()
            {
                Id = 5,
                Name = "test5"
            };
            _repository.Insert(item);
            Assert.Throws<Repository.Exceptions.UniqueKeyContraintException>(() => _repository.Insert(item));
        }

        [Fact]
        public void TestInsertRange_Get_ReturnInsertedRange()
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

            var resultGet = _repository.Get().OrderBy(x => x.Id);

            Assert.Equal(items.ElementAt(0).Key, resultGet.ElementAt(0).Key);
            Assert.Equal(items.ElementAt(0).Id, resultGet.ElementAt(0).Id);
            Assert.Equal(items.ElementAt(0).Name, resultGet.ElementAt(0).Name);

            Assert.Equal(items.ElementAt(1).Key, resultGet.ElementAt(1).Key);
            Assert.Equal(items.ElementAt(1).Id, resultGet.ElementAt(1).Id);
            Assert.Equal(items.ElementAt(1).Name, resultGet.ElementAt(1).Name);
        }

        [Fact]
        public void TestInsertRangeDuplicateKeys_ThrowsUniqueKeyContraintException()
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

            Assert.Throws<Repository.Exceptions.UniqueKeyContraintException>(() => _repository.InsertRange(items));
        }
    }
}