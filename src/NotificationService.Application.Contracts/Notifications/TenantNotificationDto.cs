using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace NotificationService.Notifications;

/// <summary>
/// Represents a published notification for a tenant/user.
/// </summary>
[Serializable]
public class TenantNotificationDto : EntityDto<Guid>, IHasCreationTime, IMultiTenant
{
    /// <summary>
    /// Tenant Id.
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Unique notification name.
    /// </summary>
    public string NotificationName { get; set; }

    /// <summary>
    /// Notification data.
    /// </summary>
    //public NotificationData Data { get; set; }

    /// <summary>
    /// Gets or sets the type of the entity.
    /// </summary>
    [Obsolete("(De)serialization of System.Type is bad and not supported. See https://github.com/dotnet/corefx/issues/42712")]
    public Type EntityType { get; set; }

    /// <summary>
    /// Name of the entity type (including namespaces).
    /// </summary>
    public string EntityTypeName { get; set; }

    /// <summary>
    /// Entity id.
    /// </summary>
    public object EntityId { get; set; }

    /// <summary>
    /// Severity.
    /// </summary>
    public NotificationSeverity Severity { get; set; }

    public DateTime CreationTime { get; set; }
}