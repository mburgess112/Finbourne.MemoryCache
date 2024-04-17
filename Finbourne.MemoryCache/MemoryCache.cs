using System.Collections.Concurrent;
using System.Xml.Linq;

namespace Finbourne.MemoryCache
{
    public class MemoryCache : IMemoryCache
    {
        private readonly int _capacity;
        private readonly ConcurrentDictionary<object, LinkedListNode<KeyValuePair<object, object>>> _keyAccessibleItems = [];
        private readonly LinkedList<KeyValuePair<object, object>> _accessOrderedItems = [];

        public MemoryCache(int capacity = 0)
        {
            _capacity = capacity;
        }

        public bool TryGetValue<T>(object key, out T? value)
        {
            if (_keyAccessibleItems.TryGetValue(key, out var storedNode))
            {
                MoveToFirst(storedNode);
                value = (T)storedNode.Value.Value;
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
            var newNode = CreateAtFirst(key, value);
            _keyAccessibleItems[key] = newNode;
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
                return item.Value.Key;
            }
        }

        private LinkedListNode<KeyValuePair<object,object>> CreateAtFirst(object key, object value)
        {
            lock (_accessOrderedItems)
            {
                return _accessOrderedItems.AddFirst(KeyValuePair.Create(key, value));
            }
        }

        private void MoveToFirst(LinkedListNode<KeyValuePair<object, object>> node)
        {
            lock (_accessOrderedItems)
            {
                _accessOrderedItems.Remove(node);
                _accessOrderedItems.AddFirst(node);
            }
        }
    }
}
