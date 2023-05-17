using System;
using Volo.Abp.DependencyInjection;

namespace NotificationService.SignalR;

// TODO: move signalr package to application layer, and change .net6.0 to net2.0
public interface IOnlineClientInfoProvider : ITransientDependency
{
    IOnlineClient CreateClientForCurrentConnection(string connectionId, Guid? tenantId, Guid? userId);
}
