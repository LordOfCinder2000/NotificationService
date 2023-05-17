namespace NotificationService.Notifications;

/// <summary>
/// Extension methods for <see cref="UserNotification"/>.
/// </summary>
public static class UserNotificationExtensions
{
    /// <summary>
    /// Converts <see cref="UserNotification"/> to <see cref="UserNotificationDto"/>.
    /// </summary>
    public static UserNotificationInfo ToUserNotification(this UserNotification userNotification, TenantNotificationInfo tenantNotificationInfo)
    {
        return new UserNotificationInfo
        {
            Id = userNotification.Id,
            Notification = tenantNotificationInfo,
            UserId = userNotification.UserId,
            State = userNotification.State,
            TenantId = userNotification.TenantId,
            TargetNotifiers = userNotification.TargetNotifiers
        };
    }
}