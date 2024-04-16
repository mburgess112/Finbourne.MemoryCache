namespace Finbourne.MemoryCache
{
    public interface IMemoryCache
    {
        void Insert(object key, object value);
        bool TryGetValue<T>(object key, out T? value);
    }
}
