using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.SignalR;

namespace NotificationService.SignalR;

public class OnlineClientHub : AbpHub
{
    protected IOnlineClientManager OnlineClientManager { get; }
    protected IOnlineClientInfoProvider OnlineClientInfoProvider { get; }

    public OnlineClientHub(
        IOnlineClientManager onlineClientManager,
        IOnlineClientInfoProvider clientInfoProvider)
    {
        OnlineClientManager = onlineClientManager;
        OnlineClientInfoProvider = clientInfoProvider;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        var client = CreateClientForCurrentConnection();

        Logger.LogDebug("A client is connected: " + client);

        await OnlineClientManager.AddAsync(client);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);

        Logger.LogDebug("A client is disconnected: " + Context.ConnectionId);

        try
        {
            await OnlineClientManager.RemoveAsync(Context.ConnectionId);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex.ToString(), ex);
        }
    }

    protected virtual IOnlineClient CreateClientForCurrentConnection()
    {
        return OnlineClientInfoProvider.CreateClientForCurrentConnection(Context.ConnectionId, CurrentTenant.Id, CurrentUser.Id);
    }
}
