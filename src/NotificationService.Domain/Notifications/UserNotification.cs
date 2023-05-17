using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace NotificationService.Notifications;

/// <summary>
/// Used to store a user notification.
/// </summary>
[Serializable]
public class UserNotification : Entity<Guid>, IHasCreationTime, IUserIdentifier
{
    /// <summary>
    /// Tenant Id.
    /// </summary>
    public virtual Guid? TenantId { get; private set; }

    /// <summary>
    /// User Id.
    /// </summary>
    public virtual Guid UserId { get; private set; }

    /// <summary>
    /// Notification Id.
    /// </summary>
    [Required]
    public virtual Guid TenantNotificationId { get; private set; }

    /// <summary>
    /// Current state of the user notification.
    /// </summary>
    public virtual UserNotificationState State { get; private set; }

    public virtual DateTime CreationTime { get; private set; }

    /// <summary>
    /// which realtime notifiers should handle this notification
    /// </summary>
    [StringLength(NotificationServiceConsts.MaxTargetNotifiersLength)]
    public virtual string TargetNotifiers { get; private set; }

    [NotMapped]
    public virtual List<string> TargetNotifiersList => TargetNotifiers.IsNullOrWhiteSpace()
        ? new List<string>()
        : TargetNotifiers.Split(NotificationServiceConsts.NotificationTargetSeparator).ToList();

    protected UserNotification()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserNotification"/> class.
    /// </summary>
    /// <param name="id"></param>
    public UserNotification(
        Guid id,
        Guid? tenantId,
        [NotNull] Guid userId,
        [NotNull] Guid tenantNotificationId,
        string targetNotifiers,
        UserNotificationState state = UserNotificationState.Unread) : base(id)
    {
        TenantId = tenantId;
        UserId = Check.NotNull(userId, nameof(userId));
        TenantNotificationId = Check.NotNull(tenantNotificationId, nameof(tenantNotificationId));
        TargetNotifiers = Check.Length(targetNotifiers, nameof(targetNotifiers), NotificationServiceConsts.MaxTargetNotifiersLength);
        SetState(state);
    }

    public virtual void SetTargetNotifiers(List<string> list)
    {
        TargetNotifiers = string.Join(NotificationServiceConsts.NotificationTargetSeparator.ToString(), list);
    }

    internal UserNotification ChangeState([NotNull] UserNotificationState state)
    {
        SetState(state);
        return this;
    }

    private void SetState([NotNull] UserNotificationState state)
    {
        if(!Enum.IsDefined(state))
        {
            throw new ArgumentException(state.ToString(), nameof(state));
        }

        State = Check.NotNull(
            state,
            nameof(state)
        );
    }
}