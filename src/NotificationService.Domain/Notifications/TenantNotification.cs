using JetBrains.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace NotificationService.Notifications;

/// <summary>
/// A notification distributed to it's related tenant.
/// </summary>
public class TenantNotification : CreationAuditedEntity<Guid>, IMultiTenant
{
    /// <summary>
    /// Tenant id of the subscribed user.
    /// </summary>
    public virtual Guid? TenantId { get; private set; }

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

    protected TenantNotification()
    {

    }

    public TenantNotification(Guid id, Guid? tenantId, [NotNull] Notification notification) : base(id)
    {
        Check.NotNull(notification, nameof(notification));
        TenantId = tenantId;
        NotificationName = Check.Length(notification.NotificationName, nameof(notification.NotificationName), NotificationServiceConsts.MaxNotificationNameLength);
        Data = Check.Length(notification.Data, nameof(notification.Data), NotificationServiceConsts.MaxDataLength);
        DataTypeName = Check.Length(notification.DataTypeName, nameof(notification.DataTypeName), NotificationServiceConsts.MaxDataTypeNameLength);
        EntityTypeName = Check.Length(notification.EntityTypeName, nameof(notification.EntityTypeName), NotificationServiceConsts.MaxEntityTypeNameLength);
        EntityTypeAssemblyQualifiedName = Check.Length(notification.EntityTypeAssemblyQualifiedName, nameof(notification.EntityTypeAssemblyQualifiedName), NotificationServiceConsts.MaxEntityTypeAssemblyQualifiedNameLength);
        EntityId = Check.Length(notification.EntityId, nameof(notification.EntityId), NotificationServiceConsts.MaxEntityIdLength);
        SetSeverity(notification.Severity);
    }

    internal TenantNotification ChangeSeverity([NotNull] NotificationSeverity severity)
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

}