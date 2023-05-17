using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace NotificationService.SignalR;

public class RedisOnlineClientStore<T> : RedisOnlineClientStore, IOnlineClientStore<T>
{
    public RedisOnlineClientStore(IDistributedCache<List<IOnlineClient>, ClientStoreCacheKey> distributedCache) 
        : base(distributedCache)
    {
    }
}

public class RedisOnlineClientStore : IOnlineClientStore, ISingletonDependency
{
    private readonly IDistributedCache<List<IOnlineClient>, ClientStoreCacheKey> _distributedCache;

    public RedisOnlineClientStore(
        IDistributedCache<List<IOnlineClient>, ClientStoreCacheKey> distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task AddAsync(IOnlineClient client)
    {
        var cacheKey = new ClientStoreCacheKey();

        var onlineClients = await _distributedCache.GetAsync(cacheKey);

        if(onlineClients == null)
        {
            onlineClients = new List<IOnlineClient>();
        }

        onlineClients.Add(client);

        await _distributedCache.SetAsync(cacheKey, onlineClients);
    }

    public async Task<bool> RemoveAsync(string connectionId)
    {
        var cacheKey = new ClientStoreCacheKey();

        var onlineClients = await _distributedCache.GetAsync(cacheKey);

        if (onlineClients.IsNullOrEmpty())
        {
            return false;
        }

        onlineClients.RemoveAll(x => x.ConnectionId.Equals(connectionId, StringComparison.InvariantCultureIgnoreCase));

        await _distributedCache.SetAsync(cacheKey, onlineClients);

        return true;
    }

    public async Task<bool> ContainsAsync(string connectionId)
    {
        var cacheKey = new ClientStoreCacheKey();

        var onlineClients = await _distributedCache.GetAsync(cacheKey);

        if (onlineClients.IsNullOrEmpty())
        {
            return false;
        }

        return onlineClients.Any(x => x.ConnectionId.Equals(connectionId, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task<IReadOnlyList<IOnlineClient>> GetAllAsync()
    {
        var cacheKey = new ClientStoreCacheKey();
        return await _distributedCache.GetAsync(cacheKey) ?? new List<IOnlineClient>();
    }

    public async Task<IOnlineClient> GetAsync(string connectionId)
    {
        var cacheKey = new ClientStoreCacheKey();

        var onlineClients = await _distributedCache.GetAsync(cacheKey);

        if (onlineClients.IsNullOrEmpty())
        {
            return null;
        }

        return onlineClients.FirstOrDefault(x => x.ConnectionId.Equals(connectionId, StringComparison.InvariantCultureIgnoreCase));
    }
}