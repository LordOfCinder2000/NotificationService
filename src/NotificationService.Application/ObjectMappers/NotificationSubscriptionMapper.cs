using NotificationService.Notifications;
using System;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Json;
using Volo.Abp.ObjectMapping;

namespace NotificationService.ObjectMappers;

public class NotificationSubscriptionMapper : IObjectMapper<NotificationSubscription, NotificationSubscriptionInfo>, ITransientDependency
{
    private readonly IJsonSerializer _jsonSerializer;

    public NotificationSubscriptionMapper(IJsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }

    public NotificationSubscriptionInfo Map(NotificationSubscription source)
    {
        var entityType = source.EntityTypeAssemblyQualifiedName.IsNullOrEmpty()
              ? null
              : Type.GetType(source.EntityTypeAssemblyQualifiedName);

        return new NotificationSubscriptionInfo
        {
            UserId = source.UserId,
            TenantId = source.TenantId,
            EntityId = source.EntityId.IsNullOrEmpty() ? null : _jsonSerializer.Deserialize(EntityHelper.FindPrimaryKeyType(entityType), source.EntityId),
            EntityType = entityType,
            EntityTypeName = source.EntityTypeName,
            NotificationName = source.NotificationName,
            CreationTime = source.CreationTime
        };
    }

    public NotificationSubscriptionInfo Map(NotificationSubscription source, NotificationSubscriptionInfo destination)
    {
        throw new NotImplementedException();
    }
}