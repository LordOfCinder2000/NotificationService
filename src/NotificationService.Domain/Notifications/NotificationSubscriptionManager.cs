using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Services;
using Volo.Abp.Json;
using Volo.Abp.ObjectMapping;

namespace NotificationService.Notifications;

/// <summary>
/// Implements <see cref="INotificationSubscriptionManager"/>.
/// </summary>
public class NotificationSubscriptionManager : DomainService, INotificationSubscriptionManager, ITransientDependency
{
    private readonly INotificationStore _store;
    private readonly INotificationDefinitionManager _notificationDefinitionManager;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly IObjectMapper _objectMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationSubscriptionManager"/> class.
    /// </summary>
    public NotificationSubscriptionManager(
        INotificationStore store,
        INotificationDefinitionManager notificationDefinitionManager,
        IJsonSerializer jsonSerializer,
        IObjectMapper objectMapper)
    {
        _store = store;
        _notificationDefinitionManager = notificationDefinitionManager;
        _jsonSerializer = jsonSerializer;
        _objectMapper = objectMapper;
    }

    public async Task SubscribeAsync(UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null)
    {
        if (await IsSubscribedAsync(user, notificationName, entityIdentifier))
        {
            return;
        }

        await _store.InsertSubscriptionAsync(
            new NotificationSubscription(
                GuidGenerator.Create(),
                user.TenantId,
                user.UserId,
                notificationName,
                entityIdentifier == null ? null : entityIdentifier.Type.FullName,
                entityIdentifier == null ? null : entityIdentifier.Type.AssemblyQualifiedName,
                entityIdentifier?.Id == null ? null : _jsonSerializer.Serialize(entityIdentifier.Id)
                )
            );
    }

    public async Task SubscribeToAllAvailableNotificationsAsync(UserIdentifier user)
    {
        var notificationDefinitions = (await _notificationDefinitionManager
            .GetAllAvailableAsync(user))
            .Where(nd => nd.EntityType == null)
            .ToList();

        foreach (var notificationDefinition in notificationDefinitions)
        {
            await SubscribeAsync(user, notificationDefinition.Name);
        }
    }

    public async Task UnsubscribeAsync(UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null)
    {
        await _store.DeleteSubscriptionAsync(
            user,
            notificationName,
            entityIdentifier == null ? null : entityIdentifier.Type.FullName,
            entityIdentifier?.Id == null ? null : _jsonSerializer.Serialize(entityIdentifier.Id)
            );
    }

    // TODO: Can work only for single database approach!
    public async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(string notificationName, EntityIdentifier entityIdentifier = null)
    {
        var notificationSubscriptions = await _store.GetSubscriptionsAsync(
            notificationName,
            entityIdentifier == null ? null : entityIdentifier.Type.FullName,
            entityIdentifier?.Id == null ? null : _jsonSerializer.Serialize(entityIdentifier.Id)
            );

        //return notificationSubscriptions
        //    //.Select(nsi => nsi.ToNotificationSubscription())
        //    .ToList();

        return _objectMapper.Map<List<NotificationSubscription>, List<NotificationSubscriptionInfo>>(notificationSubscriptions);
    }

    public async Task<List<NotificationSubscriptionInfo>> GetSubscriptionsAsync(Guid? tenantId, string notificationName, EntityIdentifier entityIdentifier = null)
    {
        var notificationSubscriptions = await _store.GetSubscriptionsAsync(
            new[] { tenantId },
            notificationName,
            entityIdentifier == null ? null : entityIdentifier.Type.FullName,
            entityIdentifier?.Id == null ? null : _jsonSerializer.Serialize(entityIdentifier.Id)
            );

        //return notificationSubscriptionInfos
        //    //.Select(nsi => nsi.ToNotificationSubscription())
        //    .ToList();

        return _objectMapper.Map<List<NotificationSubscription>, List<NotificationSubscriptionInfo>>(notificationSubscriptions);
    }

    public async Task<List<NotificationSubscriptionInfo>> GetSubscribedNotificationsAsync(UserIdentifier user)
    {
        var notificationSubscriptions = await _store.GetSubscriptionsAsync(user);

        //return notificationSubscriptionInfos
        //    //.Select(nsi => nsi.ToNotificationSubscription())
        //    .ToList();

        return _objectMapper.Map<List<NotificationSubscription>, List<NotificationSubscriptionInfo>>(notificationSubscriptions);
    }

    public Task<bool> IsSubscribedAsync(UserIdentifier user, string notificationName, EntityIdentifier entityIdentifier = null)
    {
        return _store.IsSubscribedAsync(
            user,
            notificationName,
            entityIdentifier == null ? null : entityIdentifier.Type.FullName,
            entityIdentifier?.Id == null ? null : _jsonSerializer.Serialize(entityIdentifier.Id)
            );
    }
}
