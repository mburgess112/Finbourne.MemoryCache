namespace Finbourne.MemoryCache
{
    public class MemoryCacheTests
    {
        [Fact]
        public void InsertAndGet_ItemInserted_ReturnsInsertedItem()
        {
            var cache = new MemoryCache();

            var key = "TestKey";
            var value = "TestValue";

            cache.Insert(key, value);
            cache.TryGetValue<string>(key, out var returnedValue);

            Assert.Equal(value, returnedValue);
        }

        [Fact]
        public void InsertAndGet_MultipleItemsInserted_ReturnsRequestedItem()
        {
            var cache = new MemoryCache();

            cache.Insert("FirstKey", "FirstValue");
            cache.Insert("SecondKey", "SecondValue");
            cache.Insert("ThirdKey", "ThirdValue");

            var key = "TestKey";
            var value = "TestValue";
            cache.Insert(key, value);

            cache.TryGetValue<string>(key, out var returnedValue);
            Assert.Equal(value, returnedValue);
        }

        [Fact]
        public void InsertAndGet_NoReferenceItem_ReturnsNull()
        {
            var cache = new MemoryCache();

            cache.TryGetValue<string>("TestKey", out var returnedValue);

            Assert.Null(returnedValue);
        }

        [Fact]
        public void InsertAndGet_NoValueItem_ReturnsDefault()
        {
            var cache = new MemoryCache();

            cache.TryGetValue<int>("TestKey", out var returnedValue);

            Assert.Equal(0, returnedValue);
        }

        [Fact]
        public void InsertAndGet_InvalidTypeRequested_ThrowsException()
        {
            var cache = new MemoryCache();

            var key = "TestKey";
            var value = "TestValue";

            cache.Insert(key, value);
            Assert.Throws<InvalidCastException>(() => cache.TryGetValue<int>(key, out int value));
        }

        [Fact]
        public void InsertEviction_InsertedMaxItems_DoesNotReturnOldestItem()
        {
            var cache = new MemoryCache(2);

            cache.Insert("FirstKey", "Value");
            cache.Insert("SecondKey", "SecondValue");
            cache.Insert("ThirdKey", "ThirdValue");

            cache.TryGetValue<string>("FirstKey", out var value);
            Assert.Null(value);
        }

        [Fact]
        public void InsertEviction_InsertedMaxItems_ItemsAccessed_DoesNotReturnOldestItem()
        {
            var cache = new MemoryCache(2);

            cache.Insert("FirstKey", "Value");
            cache.Insert("SecondKey", "SecondValue");
            cache.TryGetValue<string>("FirstKey", out _);
            cache.Insert("ThirdKey", "ThirdValue");

            cache.TryGetValue<string>("SecondKey", out var returnedValue);
            Assert.Null(returnedValue);
        }
    }
}
