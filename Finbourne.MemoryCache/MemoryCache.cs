namespace Finbourne.MemoryCache
{
    public class MemoryCache : IMemoryCache
    {
        private readonly int _capacity;
        private readonly Dictionary<object,  object> _keyAccessibleItems = [];
        private readonly LinkedList<object> _accessOrderedItems = [];

        public MemoryCache(int capacity = 0)
        {
            _capacity = capacity;
        }

        public bool TryGetValue<T>(object key, out T? value)
        {
            if (_keyAccessibleItems.TryGetValue(key, out var storedValue))
            {
                _accessOrderedItems.Remove(key);
                _accessOrderedItems.AddFirst(key);
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
            if (IsAtMaxCapacity())
            {
                var item = _accessOrderedItems.Last;
                _accessOrderedItems.Remove(item);
                _keyAccessibleItems.Remove(item.Value);
            }
            _accessOrderedItems.AddFirst(key);
            _keyAccessibleItems.Add(key, value);
        }

        private bool IsAtMaxCapacity()
        {
            return _capacity > 0 && _accessOrderedItems.Count == _capacity;
        }
    }
}
