using JetBrains.Annotations;
using NotificationService.Notifications;

namespace NotificationService.SignalR;

public static class OnlineClientExtensions
{
    [CanBeNull]
    public static UserIdentifier ToUserIdentifierOrNull(this IOnlineClient onlineClient)
    {
        return onlineClient.UserId.HasValue
            ? new UserIdentifier(onlineClient.TenantId, onlineClient.UserId.Value)
            : null;
    }
}