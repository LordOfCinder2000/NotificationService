using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace NotificationService.Notifications;

/// <summary>
/// Used to store a notification request.
/// This notification is distributed to tenants and users by <see cref="INotificationDistributer"/>.
/// </summary>
[Serializable]
public class Notification : CreationAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Unique notification name.
    /// </summary>
    [Required]
    [StringLength(NotificationServiceConsts.MaxNotificationNameLength)]
    public virtual string NotificationName { get; private set; }

    /// <summary>
    /// Notification data as JSON string.
    /// </summary>
    [StringLength(NotificationServiceConsts.MaxDataLength)]
    public virtual string Data { get; private set; }

    /// <summary>
    /// Type of the JSON serialized <see cref="Data"/>.
    /// It's AssemblyQualifiedName of the type.
    /// </summary>
    [StringLength(NotificationServiceConsts.MaxDataTypeNameLength)]
    public virtual string DataTypeName { get; private set; }

    /// <summary>
    /// Gets/sets entity type name, if this is an entity level notification.
    /// It's FullName of the entity type.
    /// </summary>
    [StringLength(NotificationServiceConsts.MaxEntityTypeNameLength)]
    public virtual string EntityTypeName { get; private set; }

    /// <summary>
    /// AssemblyQualifiedName of the entity type.
    /// </summary>
    [StringLength(NotificationServiceConsts.MaxEntityTypeAssemblyQualifiedNameLength)]
    public virtual string EntityTypeAssemblyQualifiedName { get; private set; }

    /// <summary>
    /// Gets/sets primary key of the entity, if this is an entity level notification.
    /// </summary>
    [StringLength(NotificationServiceConsts.MaxEntityIdLength)]
    public virtual string EntityId { get; private set; }

    /// <summary>
    /// Notification severity.
    /// </summary>
    public virtual NotificationSeverity Severity { get; private set; }

    /// <summary>
    /// Target users of the notification.
    /// If this is set, it overrides subscribed users.
    /// If this is null/empty, then notification is sent to all subscribed users.
    /// </summary>
    [StringLength(NotificationServiceConsts.MaxUserIdsLength)]
    public virtual string UserIds { get; private set; }

    /// <summary>
    /// Excluded users.
    /// This can be set to exclude some users while publishing notifications to subscribed users.
    /// It's not normally used if <see cref="UserIds"/> is not null.
    /// </summary>
    [StringLength(NotificationServiceConsts.MaxUserIdsLength)]
    public virtual string ExcludedUserIds { get; private set; }

    /// <summary>
    /// Target tenants of the notification.
    /// Used to send notification to subscribed users of specific tenant(s).
    /// This is valid only if UserIds is null.
    /// If it's "0", then indicates to all tenants.
    /// </summary>
    [StringLength(NotificationServiceConsts.MaxTenantIdsLength)]
    public virtual string TenantIds { get; private set; }

    /// <summary>
    /// which realtime notifiers should handle this notification
    /// </summary>
    [StringLength(NotificationServiceConsts.MaxTargetNotifiersLength)]
    public virtual string TargetNotifiers { get; private set; }

    [NotMapped]
    public virtual List<string> TargetNotifiersList => TargetNotifiers.IsNullOrWhiteSpace()
        ? new List<string>()
        : TargetNotifiers.Split(NotificationServiceConsts.NotificationTargetSeparator).ToList();

    protected Notification()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Notification"/> class.
    /// </summary>
    public Notification(
        Guid id,
        [NotNull] string notificationName,
        string entityTypeName,
        string entityTypeAssemblyQualifiedName,
        string entityId,
        string userIds,
        string excludedUserIds,
        string tenantId,
        string data,
        string dataTypeName,
        NotificationSeverity severity = NotificationSeverity.Info) : base(id)
    {
        NotificationName = Check.NotNullOrWhiteSpace(notificationName, nameof(notificationName), NotificationServiceConsts.MaxNotificationNameLength);
        Data = Check.Length(data, nameof(data), NotificationServiceConsts.MaxDataLength);
        DataTypeName = Check.Length(dataTypeName, nameof(dataTypeName), NotificationServiceConsts.MaxDataTypeNameLength);
        EntityTypeName = Check.Length(entityTypeName, nameof(entityTypeName), NotificationServiceConsts.MaxEntityTypeNameLength);
        EntityTypeAssemblyQualifiedName = Check.Length(entityTypeAssemblyQualifiedName, nameof(entityTypeAssemblyQualifiedName), NotificationServiceConsts.MaxEntityTypeAssemblyQualifiedNameLength);
        EntityId = Check.Length(entityId, nameof(entityId), NotificationServiceConsts.MaxEntityIdLength);
        UserIds = Check.Length(userIds, nameof(userIds), NotificationServiceConsts.MaxUserIdsLength);
        ExcludedUserIds = Check.Length(excludedUserIds, nameof(excludedUserIds), NotificationServiceConsts.MaxUserIdsLength);
        TenantIds = Check.Length(tenantId, nameof(tenantId), NotificationServiceConsts.MaxTenantIdsLength);

        SetSeverity(severity);
    }
    internal Notification ChangeSeverity([NotNull] NotificationSeverity severity)
    {
        SetSeverity(severity);
        return this;
    }

    private void SetSeverity([NotNull] NotificationSeverity severity)
    {
        if (!Enum.IsDefined(severity))
        {
            throw new ArgumentException(severity.ToString(), nameof(severity));
        }

        Severity = Check.NotNull(
            severity,
            nameof(severity)
        );
    }

    public virtual void SetTargetNotifiers(List<string> list)
    {
        TargetNotifiers = string.Join(NotificationServiceConsts.NotificationTargetSeparator.ToString(), list);
    }
}