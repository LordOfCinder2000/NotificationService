using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace NotificationService.SignalR;

public class OnlineClientInfoProvider : IOnlineClientInfoProvider, ITransientDependency
{
    private readonly IClientInfoProvider _clientInfoProvider;
    private readonly IClock _clock;

    public ILogger<OnlineClientInfoProvider> Logger { get; set; }

    public OnlineClientInfoProvider(
        IClientInfoProvider clientInfoProvider,
        IClock clock
        )
    {
        _clientInfoProvider = clientInfoProvider;
        _clock = clock;
        Logger = NullLogger<OnlineClientInfoProvider>.Instance;
    }

    public IOnlineClient CreateClientForCurrentConnection(string connectionId, Guid? tenantId, Guid? userId)
    {
        return new OnlineClient(
            connectionId,
            GetIpAddressOfClient(connectionId),
            tenantId,
            userId,
            _clock
        );
    }

    private string GetIpAddressOfClient(string connectionId)
    {
        try
        {
            return _clientInfoProvider.ClientIpAddress;
        }
        catch (Exception ex)
        {
            Logger.LogError("Can not find IP address of the client! connectionId: " + connectionId);
            Logger.LogError(ex.Message, ex);
            return "";
        }
    }
}
