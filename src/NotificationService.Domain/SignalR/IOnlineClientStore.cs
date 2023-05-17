using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationService.SignalR;

public interface IOnlineClientStore<T> : IOnlineClientStore
{

}

public interface IOnlineClientStore
{
    /// <summary>
    /// Get a client.
    /// </summary>
    /// <param name="client">The client.</param>
    Task<IOnlineClient> GetAsync(string connectionId);

    /// <summary>
    /// Adds a client.
    /// </summary>
    /// <param name="client">The client.</param>
    Task AddAsync(IOnlineClient client);

    /// <summary>
    /// Removes a client by connection id.
    /// </summary>
    /// <param name="connectionId">The connection id.</param>
    /// <returns>true if the client is removed, otherwise, false</returns>
    Task<bool> RemoveAsync(string connectionId);

    /// <summary>
    /// Determines if store contains client with connection id.
    /// </summary>
    /// <param name="connectionId">The connection id.</param>
    Task<bool> ContainsAsync(string connectionId);

    /// <summary>
    /// Gets all online clients.
    /// </summary>
    Task<IReadOnlyList<IOnlineClient>> GetAllAsync();
}
