using JetBrains.Annotations;
using NotificationService.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace NotificationService.SignalR;

public class OnlineClientManager<T> : OnlineClientManager, IOnlineClientManager<T>
{
    public OnlineClientManager(IOnlineClientStore<T> store) : base(store)
    {

    }
}

/// <summary>
/// Implements <see cref="IOnlineClientManager"/>.
/// </summary>
public class OnlineClientManager : IOnlineClientManager, ISingletonDependency
{
    /// <summary>
    /// Online clients Store.
    /// </summary>
    protected IOnlineClientStore Store { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlineClientManager"/> class.
    /// </summary>
    public OnlineClientManager(IOnlineClientStore store)
    {
        Store = store;
    }

    public virtual async Task AddAsync(IOnlineClient client)
    {
        var userWasAlreadyOnline = false;
        var user = client.ToUserIdentifierOrNull();

        if (user != null)
        {
            userWasAlreadyOnline = (await GetAllByUserIdAsync(user)).Any();
        }

        await Store.AddAsync(client);

        //ClientConnected?.Invoke(this, new OnlineClientEventArgs(client));

        if (user != null && !userWasAlreadyOnline)
        {
            //UserConnected?.Invoke(this, new OnlineUserEventArgs(user, client));
        }
    }

    public virtual async Task<bool> RemoveAsync(string connectionId)
    {
        var result = await Store.RemoveAsync(connectionId);
        if (!result)
        {
            return false;
        }

        //if (UserDisconnected != null)
        //{
        //    var user = client.ToUserIdentifierOrNull();

        //    if (user != null && !this.IsOnline(user))
        //    {
        //        UserDisconnected.Invoke(this, new OnlineUserEventArgs(user, client));
        //    }
        //}

        //ClientDisconnected?.Invoke(this, new OnlineClientEventArgs(client));

        return true;
    }

    public virtual async Task<IOnlineClient> GetByConnectionIdOrNullAsync(string connectionId)
    {
        return await Store.GetAsync(connectionId);
    }

    public virtual async Task<IReadOnlyList<IOnlineClient>> GetAllClientsAsync()
    {
        return await Store.GetAllAsync();
    }

    [NotNull]
    public virtual async Task<IReadOnlyList<IOnlineClient>> GetAllByUserIdAsync([NotNull] IUserIdentifier user)
    {
        Check.NotNull(user, nameof(user));

        return (await GetAllClientsAsync())
             .Where(c => c.UserId == user.UserId && c.TenantId == user.TenantId)
             .ToImmutableList();
    }
}
