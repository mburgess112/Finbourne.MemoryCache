using System.Collections.Concurrent;

namespace Finbourne.MemoryCache
{
    public class MemoryCache : IMemoryCache
    {
        private readonly int _capacity;
        private readonly ConcurrentDictionary<object,  object> _keyAccessibleItems = [];
        private readonly LinkedList<object> _accessOrderedItems = [];

        public MemoryCache(int capacity = 0)
        {
            _capacity = capacity;
        }

        public bool TryGetValue<T>(object key, out T? value)
        {
            if (_keyAccessibleItems.TryGetValue(key, out var storedValue))
            {
                SetMostRecentItem(key);
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
            EnsureCapacity();
            _accessOrderedItems.AddFirst(key);
            _keyAccessibleItems[key] = value;
        }

        private void EnsureCapacity()
        {
            if (_capacity == 0) { return; }

            if (_accessOrderedItems.Count < _capacity)
            {
                return;
            }
            var keyToEvict = TryGetKeyToEvict();
            if (keyToEvict != null)
            {
                _keyAccessibleItems.Remove(keyToEvict, out _);
            }
        }

        private object? TryGetKeyToEvict()
        {
            lock (_accessOrderedItems)
            {
                if (_accessOrderedItems.Count < _capacity) 
                {
                    return null;
                };
                var item = _accessOrderedItems.Last;
                if (item == null) 
                { 
                    return null; 
                }
                _accessOrderedItems.Remove(item);
                return item.Value;
            }
        }

        private void SetMostRecentItem(object key)
        {
            lock (_accessOrderedItems)
            {
                _accessOrderedItems.Remove(key);
                _accessOrderedItems.AddFirst(key);
            }
        }
    }
}
