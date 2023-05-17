using JetBrains.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace NotificationService.Notifications;

/// <summary>
/// Used to store a notification subscription.
/// </summary>
public class NotificationSubscription : CreationAuditedAggregateRoot<Guid>, IUserIdentifier
{
    /// <summary>
    /// Tenant id of the subscribed user.
    /// </summary>
    public virtual Guid? TenantId { get; private set; }

    /// <summary>
    /// User Id.
    /// </summary>
    public virtual Guid UserId { get; private set; }

    /// <summary>
    /// Notification unique name.
    /// </summary>
    [StringLength(NotificationServiceConsts.MaxNotificationNameLength)]
    public virtual string NotificationName { get; private set; }

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
    /// Initializes a new instance of the <see cref="NotificationSubscription"/> class.
    /// </summary>
    protected NotificationSubscription()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationSubscription"/> class.
    /// </summary>
    public NotificationSubscription(
        Guid id,
        Guid? tenantId,
        [NotNull] Guid userId,
        string notificationName,
        string entityTypeName,
        string entityTypeAssemblyQualifiedName,
        string entityId) : base(id)
    {
        TenantId = tenantId;
        NotificationName = Check.Length(notificationName, nameof(notificationName), NotificationServiceConsts.MaxNotificationNameLength);
        UserId = Check.NotNull(userId, nameof(userId));
        EntityTypeName = Check.Length(entityTypeName, nameof(entityTypeName), NotificationServiceConsts.MaxEntityTypeNameLength);
        EntityTypeAssemblyQualifiedName = Check.Length(entityTypeAssemblyQualifiedName, nameof(entityTypeAssemblyQualifiedName), NotificationServiceConsts.MaxEntityTypeAssemblyQualifiedNameLength);
        EntityId = Check.Length(entityId, nameof(entityId), NotificationServiceConsts.MaxEntityIdLength);
    }
}
