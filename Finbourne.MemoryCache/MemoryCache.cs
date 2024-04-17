using System.Collections.Concurrent;

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
                UpdateItemAccess(storedNode);
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
            if (TryGetItemToEvict(out var itemToEvict))
            {
                _keyAccessibleItems.Remove(itemToEvict.Key, out _);
            }
            var newNode = CreateItem(KeyValuePair.Create(key, value));
            _keyAccessibleItems[key] = newNode;
        }

        private LinkedListNode<KeyValuePair<object, object>> CreateItem(KeyValuePair<object, object> value)
        {
            lock (_accessOrderedItems)
            {
                return _accessOrderedItems.AddFirst(value);
            }
        }

        private void UpdateItemAccess(LinkedListNode<KeyValuePair<object, object>> node)
        {
            lock (_accessOrderedItems)
            {
                _accessOrderedItems.Remove(node);
                _accessOrderedItems.AddFirst(node);
            }
        }

        private bool TryGetItemToEvict(out KeyValuePair<object, object> itemToEvict)
        {
            itemToEvict = default;
            if (_capacity == 0 || _accessOrderedItems.Count < _capacity) 
            { 
                return false; 
            }

            lock (_accessOrderedItems)
            {
                if (_accessOrderedItems.Count < _capacity) 
                {
                    return false;
                };
                var node = _accessOrderedItems.Last;
                if (node == null) 
                {
                    return false; 
                }
                _accessOrderedItems.Remove(node);
                itemToEvict = node.Value;
                return true;
            }
        }
    }
}
