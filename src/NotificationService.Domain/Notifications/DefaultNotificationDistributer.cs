using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Options;
using NotificationService.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectMapping;
using Volo.Abp.SettingManagement;
using Volo.Abp.Uow;

namespace NotificationService.Notifications;

/// <summary>
/// Used to distribute notifications to users.
/// </summary>
public class DefaultNotificationDistributer : DomainService, INotificationDistributer
{
    private readonly NotificationServiceOptions _notificationServiceOptions;
    private readonly INotificationDefinitionManager _notificationDefinitionManager;
    private readonly INotificationStore _notificationStore;
    private readonly ISettingManager _settingManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IObjectMapper _objectMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationDistributionJob"/> class.
    /// </summary>
    public DefaultNotificationDistributer(
        IOptions<NotificationServiceOptions> notificationServiceOptions,
        INotificationDefinitionManager notificationDefinitionManager,
        INotificationStore notificationStore,
        ISettingManager settingManager,
        IUnitOfWorkManager unitOfWorkManager,
        IObjectMapper objectMapper)
    {
        _notificationServiceOptions = notificationServiceOptions.Value;
        _notificationDefinitionManager = notificationDefinitionManager;
        _notificationStore = notificationStore;
        _settingManager = settingManager;
        _unitOfWorkManager = unitOfWorkManager;
        _objectMapper = objectMapper;
    }

    public virtual async Task DistributeAsync(Guid notificationId)
    {
        var notification = await _notificationStore.GetNotificationOrNullAsync(notificationId);
        if (notification == null)
        {
            Logger.LogWarning(
                "NotificationDistributionJob can not continue since could not found notification by id: " +
                notificationId
            );

            return;
        }

        var users = await GetUsersAsync(notification);

        var userNotificationInfos = await SaveUserNotificationsAsync(users, notification);

        await _notificationStore.DeleteNotificationAsync(notification);

        await NotifyAsync(userNotificationInfos.ToArray());
    }

    protected virtual async Task<UserIdentifier[]> GetUsersAsync(Notification notification)
    {
        List<UserIdentifier> userIds;

        if (!notification.UserIds.IsNullOrEmpty())
        {
            //Directly get from UserIds
            userIds = notification
                .UserIds
                .Split(",")
                .Select(uidAsStr => UserIdentifier.Parse(uidAsStr))
                .ToList();

            foreach (var item in userIds)
            {
                using (CurrentTenant.Change(item.TenantId))
                {
                    var setting = await _settingManager.GetOrNullForUserAsync(NotificationServiceSettings.Notification.ReceiveNotifications, item.UserId);
                    if (setting.IsNullOrWhiteSpace())
                    {
                        userIds.Remove(item);
                    }
                }
            }
        }
        else
        {
            //Get subscribed users

            var tenantIds = GetTenantIds(notification);

            List<NotificationSubscription> subscriptions;

            if (tenantIds.IsNullOrEmpty() ||
                (tenantIds.Length == 1 && tenantIds[0] == NotificationServiceConsts.AllTenantIds.To<Guid>()))
            {
                //Get all subscribed users of all tenants
                subscriptions = await _notificationStore.GetSubscriptionsAsync(
                    notification.NotificationName,
                    notification.EntityTypeName,
                    notification.EntityId
                );
            }
            else
            {
                //Get all subscribed users of specified tenant(s)
                subscriptions = await _notificationStore.GetSubscriptionsAsync(
                    tenantIds,
                    notification.NotificationName,
                    notification.EntityTypeName,
                    notification.EntityId
                );
            }

            //Remove invalid subscriptions
            var invalidSubscriptions = new Dictionary<Guid, NotificationSubscription>();

            //TODO: Group subscriptions per tenant for potential performance improvement
            foreach (var subscription in subscriptions)
            {
                using (CurrentTenant.Change(subscription.TenantId))
                {
                    var setting = await _settingManager.GetOrNullForUserAsync(NotificationServiceSettings.Notification.ReceiveNotifications, subscription.UserId);
                    if (!await _notificationDefinitionManager.IsAvailableAsync(notification.NotificationName,
                            new UserIdentifier(subscription.TenantId, subscription.UserId)) ||
                        setting.IsNullOrWhiteSpace())
                    {
                        invalidSubscriptions[subscription.Id] = subscription;
                    }
                }
            }

            subscriptions.RemoveAll(s => invalidSubscriptions.ContainsKey(s.Id));

            //Get user ids
            userIds = subscriptions
                .Select(s => new UserIdentifier(s.TenantId, s.UserId))
                .ToList();
        }

        if (!notification.ExcludedUserIds.IsNullOrEmpty())
        {
            //Exclude specified users.
            var excludedUserIds = notification
                .ExcludedUserIds
                .Split(",")
                .Select(uidAsStr => UserIdentifier.Parse(uidAsStr))
                .ToList();

            userIds.RemoveAll(uid => excludedUserIds.Any(euid => euid.Equals(uid)));
        }

        return userIds.ToArray();
    }

    private static Guid?[] GetTenantIds(Notification notification)
    {
        if (notification.TenantIds.IsNullOrEmpty())
        {
            return null;
        }

        return notification
            .TenantIds
            .Split(",")
            .Select(tenantIdAsStr => tenantIdAsStr == "null" ? null : (Guid?)tenantIdAsStr.To<Guid>())
            .ToArray();
    }

    protected virtual async Task<List<UserNotificationInfo>> SaveUserNotificationsAsync(
        UserIdentifier[] users,
        Notification notification)
    {
        var userNotificationInfos = new List<UserNotificationInfo>();

        var tenantGroups = users.GroupBy(user => user.TenantId);
        foreach (var tenantGroup in tenantGroups)
        {
            using (CurrentTenant.Change(tenantGroup.Key))
            {
                var tenantNotification = new TenantNotification(GuidGenerator.Create(),
                    tenantGroup.Key, notification);
                await _notificationStore.InsertTenantNotificationAsync(tenantNotification);
                await _unitOfWorkManager.Current.SaveChangesAsync(); //To get tenantNotification.Id.

                //var tenantNotification = tenantNotificationInfo.ToTenantNotification();
                var tenantNotificationInfo = _objectMapper.Map<TenantNotification, TenantNotificationInfo>(tenantNotification);

                foreach (var user in tenantGroup)
                {
                    var userNotification = new UserNotification(
                        GuidGenerator.Create(),
                        tenantGroup.Key,
                        user.UserId,
                        tenantNotification.Id,
                        notification.TargetNotifiers);

                    await _notificationStore.InsertUserNotificationAsync(userNotification);
                    userNotificationInfos.Add(userNotification.ToUserNotification(tenantNotificationInfo));
                }

                await _unitOfWorkManager.Current.SaveChangesAsync(); //To get Ids of the notifications
            }
        }

        return userNotificationInfos;
    }

    #region Protected methods

    protected virtual async Task NotifyAsync(UserNotificationInfo[] userNotificationInfos)
    {
        foreach (var notifierType in _notificationServiceOptions.Configuration.Notifiers)
        {
            try
            {
                var notifier = (IRealTimeNotifier)LazyServiceProvider.LazyGetRequiredService(notifierType);
                UserNotificationInfo[] notificationsToSendWithThatNotifier;

                // if UseOnlyIfRequestedAsTarget is true, then we should send notifications which requests this notifier
                if (notifier.UseOnlyIfRequestedAsTarget)
                {
                    notificationsToSendWithThatNotifier = userNotificationInfos
                        .Where(n => n.TargetNotifiersList.Contains(notifierType.FullName))
                        .ToArray();
                }
                else
                {
                    // notifier allows to send any notifications 
                    // we can send all notifications which does not have TargetNotifiersList(since there is no target, we can send it with any notifier)
                    // or current notifier is in TargetNotifiersList

                    notificationsToSendWithThatNotifier = userNotificationInfos
                        .Where(n =>
                                n.TargetNotifiersList == null || n.TargetNotifiersList.Count == 0 ||// if there is no target notifiers, send it to all of them
                                n.TargetNotifiersList.Contains(notifierType.FullName)// if there is target notifiers, check if current notifier is in it
                        )
                        .ToArray();
                }

                if (notificationsToSendWithThatNotifier.Length == 0)
                {
                    continue;
                }

                await notifier.SendNotificationsAsync(notificationsToSendWithThatNotifier);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString(), ex);
            }
        }
    }

    #endregion
}