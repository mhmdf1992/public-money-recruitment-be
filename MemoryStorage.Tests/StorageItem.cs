using Repository;
namespace MemoryStorage.Tests
{
    public class StorageItem : Identified
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key => Id.ToString();
    }
}
