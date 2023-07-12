using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;

namespace Cache.Operations;

public interface ICacheServicce
{
    ICacheClient<TKey, TValue> CreateCache<TKey, TValue>(string cacheName);
    ICacheClient<TKey, TValue> GetOrCreateCache<TKey, TValue>(CacheClientConfiguration configuration);
    ICacheClient<TKey, TValue> GetCache<TKey, TValue>(string cacheName);
    void DeleteCache(string cacheName);
    void Put<TKey, TValue>(string cacheName, TKey key, TValue value);
    void PutAll<TKey, TValue>(string cacheName, Dictionary<TKey, TValue> keyValuePair);
    TValue GetValue<TKey, TValue>(string cacheName, TKey key);
    IFieldsQueryCursor? ExecuteQuery<Tkey, TValue>(string cacheName, string query);
    void RemoveAll<TKey, TValue>(string cacheName, IEnumerable<TKey> keys);
    void RemoveAll<TKey, TValue>(string cacheName);
    long GetCacheSize<TKey, TValue>(string cacheName);
}

public class CacheService : ICacheServicce
{
    private readonly IIgniteClient _client;
    public CacheService(IClient client)
    {
        _client = client.Client();
    }

    public ICacheClient<TKey, TValue> CreateCache<TKey, TValue>(string cacheName)
    {
        return _client.CreateCache<TKey, TValue>(cacheName);
    }

    public ICacheClient<TKey, TValue> GetOrCreateCache<TKey, TValue>(CacheClientConfiguration configuration)
    {
        return _client.GetOrCreateCache<TKey, TValue>(configuration);
    }

    public ICacheClient<TKey, TValue> GetCache<TKey, TValue>(string cacheName)
    {
        return _client.GetCache<TKey, TValue>(cacheName);
    }

    public void DeleteCache(string cacheName)
    {
        _client.DestroyCache(cacheName);
    }

    public void Put<TKey, TValue>(string cacheName, TKey key, TValue value)
    {
        var cache = _client.GetCache<TKey, TValue>(cacheName);
        cache.Put(key, value);
    }

    public void PutAll<TKey, TValue>(string cacheName, Dictionary<TKey, TValue> keyValuePair)
    {
        var cache = _client.GetCache<TKey, TValue>(cacheName);
        cache.PutAll(keyValuePair);
    }

    public TValue GetValue<TKey, TValue>(string cacheName, TKey key)
    {
        var cache = _client.GetCache<TKey, TValue>(cacheName);
        return cache.Get(key);
    }

    public IFieldsQueryCursor? ExecuteQuery<Tkey, TValue>(string cacheName, string query)
    {
        var sql = new SqlFieldsQuery(query);
        sql.EnableDistributedJoins = true;

        var cache = _client.GetCache<Tkey, TValue>(cacheName);
        var result = cache.Query(sql);

        return result;
    }

    public void RemoveAll<TKey, TValue>(string cacheName, IEnumerable<TKey> keys)
    {
        var cache = _client.GetCache<TKey, TValue>(cacheName);
        cache.RemoveAll(keys);
    }

    public void RemoveAll<TKey, TValue>(string cacheName)
    {
        var cache = _client.GetCache<TKey, TValue>(cacheName);
        cache.RemoveAll();
    }

    public long GetCacheSize<TKey, TValue>(string cacheName)
    {
        var cache = _client.GetCache<TKey, TValue>(cacheName);

        return cache.GetSize();
    }
}
