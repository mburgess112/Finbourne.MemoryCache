namespace Finbourne.MemoryCache
{
    public class MemoryCache : IMemoryCache
    {
        private readonly Dictionary<object,  object> _cache = [];

        public bool TryGetValue<T>(object key, out T? value)
        {
            if (_cache.TryGetValue(key, out var storedValue))
            {
                value = (T)storedValue;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public void Insert(object key, object value)
        {
            _cache.Add(key, value);
        }
    }
}
