namespace NotificationService.Notifications;

/// <summary>
/// A class contains a <see cref="Notifications.UserNotification"/> and related <see cref="Notifications.Notification"/>.
/// </summary>
public class UserNotificationWithNotification
{
    /// <summary>
    /// User notification.
    /// </summary>
    public UserNotification UserNotification { get; set; }

    /// <summary>
    /// Notification.
    /// </summary>
    public TenantNotification Notification { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserNotificationWithNotification"/> class.
    /// </summary>
    public UserNotificationWithNotification(UserNotification userNotification, TenantNotification notification)
    {
        UserNotification = userNotification;
        Notification = notification;
    }
}