using NotificationService.Notifications;
using System;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace NotificationService.ObjectMappers;

public class UserNotificationWithNotificationMapper : IObjectMapper<UserNotificationWithNotification, UserNotificationInfo>, ITransientDependency
{
    private readonly IObjectMapper _objectMapper;

    public UserNotificationWithNotificationMapper(IObjectMapper objectMapper)
    {
        _objectMapper = objectMapper;
    }

    public UserNotificationInfo Map(UserNotificationWithNotification source)
    {
        var tenantNotificationInfo = _objectMapper.Map<TenantNotification, TenantNotificationInfo>(source.Notification);

        return source.UserNotification.ToUserNotification(tenantNotificationInfo);
    }

    public UserNotificationInfo Map(UserNotificationWithNotification source, UserNotificationInfo destination)
    {
        throw new NotImplementedException();
    }
}