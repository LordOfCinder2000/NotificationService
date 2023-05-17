using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Auditing;

namespace NotificationService.Notifications;

/// <summary>
/// Represents a notification sent to a user.
/// </summary>
[Serializable]
public class UserNotificationInfo : IHasCreationTime, IUserIdentifier
{
    /// <summary>
    /// TenantId.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// TenantId.
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// User Id.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Current state of the user notification.
    /// </summary>
    public UserNotificationState State { get; set; }

    /// <summary>
    /// The notification.
    /// </summary>
    public TenantNotificationInfo Notification { get; set; }

    /// <summary>
    /// which realtime notifiers should handle this notification
    /// </summary>
    public string TargetNotifiers { get; set; }

    public List<string> TargetNotifiersList => TargetNotifiers.IsNullOrWhiteSpace()
        ? new List<string>()
        : TargetNotifiers.Split(NotificationServiceConsts.NotificationTargetSeparator).ToList();

    public DateTime CreationTime { get; set; }
}