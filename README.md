# Finbourne.MemoryCache
Features:
1. Insert and retrieve items with arbitrary types.
1. Least-recently used item eviction on insert.
1. Concurrent access.

## Notes
* Initial interface based on the dotnet IMemoryCache: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory.imemorycache?view=dotnet-plat-ext-8.0
* Based on where the concurrent-insert code has gone, I think it makes sense to keep a separation between that method and try-get. That said, if I'd started with the ConcurrentDictionary under the hood it may have been nice to target some of its other methods directly for GetOrInsert operations.
* The idea for using the linked list to implement LRU came from this link at the top of Google: https://www.interviewcake.com/concept/java/lru-cache
* I can draw a box-diagram to explain why the double-links are good for accessing first/last items
* With a little more refactoring, the LRU implementation could be extracted as a separate class, and replaced with other strategies for knowing what to evict.
* Direct unit testing of the eviction part would be handy; I initially had a bug where the list retained multiple references to keys that were updated.
* Time: I spent about 4 hours on the exercise overall. Full disclosure - I got the exercise spec on Monday lunchtime and went through it Tuesday/Wednesday evenings, so the problem was in the back of my mind for a while.