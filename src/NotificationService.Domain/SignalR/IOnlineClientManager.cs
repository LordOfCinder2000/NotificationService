using JetBrains.Annotations;
using NotificationService.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationService.SignalR;

/// <summary>
/// Used to manage online clients those are connected to the application.
/// </summary>
public interface IOnlineClientManager<T> : IOnlineClientManager
{

}

public interface IOnlineClientManager
{
    /// <summary>
    /// Adds a client.
    /// </summary>
    /// <param name="client">The client.</param>
    Task AddAsync(IOnlineClient client);

    /// <summary>
    /// Removes a client by connection id.
    /// </summary>
    /// <param name="connectionId">The connection id.</param>
    /// <returns>True, if a client is removed</returns>
    Task<bool> RemoveAsync(string connectionId);

    /// <summary>
    /// Tries to find a client by connection id.
    /// Returns null if not found.
    /// </summary>
    /// <param name="connectionId">connection id</param>
    Task<IOnlineClient> GetByConnectionIdOrNullAsync(string connectionId);

    /// <summary>
    /// Gets all online clients.
    /// </summary>
    Task<IReadOnlyList<IOnlineClient>> GetAllClientsAsync();

    /// <summary>
    /// Gets all online clients by user id.
    /// </summary>
    /// <param name="user">user identifier</param>
    Task<IReadOnlyList<IOnlineClient>> GetAllByUserIdAsync([NotNull] IUserIdentifier user);
}