using NotificationService.Notifications;
using System;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Json;
using Volo.Abp.ObjectMapping;

namespace NotificationService.ObjectMappers;

public class TenantNotificationMapper : IObjectMapper<TenantNotification, TenantNotificationInfo>, ITransientDependency
{
    private readonly IJsonSerializer _jsonSerializer;

    public TenantNotificationMapper(IJsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }

    public TenantNotificationInfo Map(TenantNotification source)
    {
        var entityType = source.EntityTypeAssemblyQualifiedName.IsNullOrEmpty()
                ? null
                : Type.GetType(source.EntityTypeAssemblyQualifiedName);
        return new TenantNotificationInfo
        {
            Id = source.Id,
            TenantId = source.TenantId,
            EntityId = source.EntityId.IsNullOrEmpty() ? null : _jsonSerializer.Deserialize(EntityHelper.FindPrimaryKeyType(entityType), source.EntityId),
#pragma warning disable CS0618 // Type or member is obsolete, this line will be removed once the EntityType property is removed
            EntityType = entityType,
            Data = source.Data.IsNullOrEmpty() ? null : _jsonSerializer.Deserialize(Type.GetType(source.DataTypeName), source.Data) as NotificationData,
            EntityTypeName = source.EntityTypeName,
            NotificationName = source.NotificationName,
            Severity = source.Severity,
            CreationTime = source.CreationTime
        };
    }

    public TenantNotificationInfo Map(TenantNotification source, TenantNotificationInfo destination)
    {
        throw new NotImplementedException();
    }
}