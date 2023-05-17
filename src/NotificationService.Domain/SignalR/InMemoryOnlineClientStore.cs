using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace NotificationService.SignalR;

public class InMemoryOnlineClientStore<T> : InMemoryOnlineClientStore, IOnlineClientStore<T>
{
}

public class InMemoryOnlineClientStore : IOnlineClientStore, ISingletonDependency
{
    /// <summary>
    /// Online clients.
    /// </summary>
    protected ConcurrentDictionary<string, IOnlineClient> Clients { get; }

    public InMemoryOnlineClientStore()
    {
        Clients = new ConcurrentDictionary<string, IOnlineClient>();
    }

    public Task<IOnlineClient> GetAsync(string connectionId)
    {
        return Task.FromResult(Clients.GetValueOrDefault(connectionId));
    }

    public Task AddAsync(IOnlineClient client)
    {
        Clients.AddOrUpdate(client.ConnectionId, client, (s, o) => client);

        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(string connectionId)
    {
        return Task.FromResult(Clients.TryRemove(connectionId, out var client));
    }

    public Task<bool> ContainsAsync(string connectionId)
    {
        return Task.FromResult(Clients.ContainsKey(connectionId));
    }

    public Task<IReadOnlyList<IOnlineClient>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyList<IOnlineClient>>(Clients.Values.ToImmutableList());
    }
}
