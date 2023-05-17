using NotificationService.Notifications;
using Volo.Abp.Collections;

namespace NotificationService.Options;

/// <summary>
/// Used to configure notification system.
/// </summary>
public interface INotificationConfiguration
{
    /// <summary>
    /// Notification providers.
    /// </summary>
    ITypeList<NotificationProvider> Providers { get; }

    /// <summary>
    /// A list of contributors for notification notifying process.
    /// </summary>
    ITypeList<IRealTimeNotifier> Notifiers { get; }
}

