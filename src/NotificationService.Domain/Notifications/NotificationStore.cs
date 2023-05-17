using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Linq;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace NotificationService.Notifications;

/// <summary>
/// Implements <see cref="INotificationStore"/> using repositories.
/// </summary>
public class NotificationStore : DomainService, INotificationStore, ITransientDependency
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ITenantNotificationRepository _tenantNotificationRepository;
    private readonly IUserNotificationRepository _userNotificationRepository;
    private readonly INotificationSubscriptionRepository _notificationSubscriptionRepository;
    private readonly IDataFilter _dataFilter;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationStore"/> class.
    /// </summary>
    public NotificationStore(
        INotificationRepository notificationRepository,
        ITenantNotificationRepository tenantNotificationRepository,
        IUserNotificationRepository userNotificationRepository,
        INotificationSubscriptionRepository notificationSubscriptionRepository,
        IDataFilter dataFilter)
    {
        _notificationRepository = notificationRepository;
        _tenantNotificationRepository = tenantNotificationRepository;
        _userNotificationRepository = userNotificationRepository;
        _notificationSubscriptionRepository = notificationSubscriptionRepository;
        _dataFilter = dataFilter;
    }

    public virtual async Task InsertSubscriptionAsync(NotificationSubscription subscription)
    {
        using (CurrentTenant.Change(subscription.TenantId))
        {
            await _notificationSubscriptionRepository.InsertAsync(subscription);
        }
    }

    public virtual async Task DeleteSubscriptionAsync(
        UserIdentifier user,
        string notificationName,
        string entityTypeName,
        string entityId)
    {
        using (CurrentTenant.Change(user.TenantId))
        {
            await _notificationSubscriptionRepository.DeleteAsync(s =>
                s.UserId == user.UserId &&
                s.NotificationName == notificationName &&
                s.EntityTypeName == entityTypeName &&
                s.EntityId == entityId
            );
        }
    }

    public virtual async Task InsertNotificationAsync(Notification notification)
    {
        await _notificationRepository.InsertAsync(notification);
    }

    public virtual async Task<Notification> GetNotificationOrNullAsync(Guid notificationId)
    {
        return await _notificationRepository.FindAsync(notificationId);
    }

    public virtual async Task InsertUserNotificationAsync(UserNotification userNotification)
    {
        using (CurrentTenant.Change(userNotification.TenantId))
        {
            await _userNotificationRepository.InsertAsync(userNotification);
        }
    }

    public virtual async Task<List<NotificationSubscription>> GetSubscriptionsAsync(
        string notificationName,
        string entityTypeName,
        string entityId)
    {
        using (_dataFilter.Disable<IMultiTenant>())
        {
            return await _notificationSubscriptionRepository.GetListAsync(s =>
                s.NotificationName == notificationName &&
                s.EntityTypeName == entityTypeName &&
                s.EntityId == entityId
            );
        }
    }

    public virtual async Task<List<NotificationSubscription>> GetSubscriptionsAsync(
        Guid?[] tenantIds,
        string notificationName,
        string entityTypeName,
        string entityId)
    {
        var subscriptions = new List<NotificationSubscription>();

        foreach (var tenantId in tenantIds)
        {
            subscriptions.AddRange(await GetSubscriptionsAsync(tenantId, notificationName, entityTypeName,
                entityId));
        }

        return subscriptions;
    }

    public virtual async Task<List<NotificationSubscription>> GetSubscriptionsAsync(UserIdentifier user)
    {
        using (CurrentTenant.Change(user.TenantId))
        {
            return await _notificationSubscriptionRepository.GetListAsync(s => s.UserId == user.UserId);
        }
    }

    protected virtual async Task<List<NotificationSubscription>> GetSubscriptionsAsync(
        Guid? tenantId,
        string notificationName,
        string entityTypeName,
        string entityId)
    {
        using (CurrentTenant.Change(tenantId))
        {
            return await _notificationSubscriptionRepository.GetListAsync(s =>
                s.NotificationName == notificationName &&
                s.EntityTypeName == entityTypeName &&
                s.EntityId == entityId
            );
        }
    }

    public virtual async Task<bool> IsSubscribedAsync(
        UserIdentifier user,
        string notificationName,
        string entityTypeName,
        string entityId)
    {
        using (CurrentTenant.Change(user.TenantId))
        {
            return await _notificationSubscriptionRepository.LongCountAsync(s =>
                s.UserId == user.UserId &&
                s.NotificationName == notificationName &&
                s.EntityTypeName == entityTypeName &&
                s.EntityId == entityId
            ) > 0;
        }
    }

    public virtual async Task UpdateUserNotificationStateAsync(
        Guid? tenantId,
        Guid userNotificationId,
        UserNotificationState state)
    {
        using (CurrentTenant.Change(tenantId))
        {
            var userNotification = await _userNotificationRepository.FindAsync(userNotificationId);
            if (userNotification == null)
            {
                return;
            }

            userNotification.ChangeState(state);
        }
    }

    public virtual async Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
    {
        using (CurrentTenant.Change(user.TenantId))
        {
            var userNotifications = await _userNotificationRepository.GetListAsync(
                un => un.UserId == user.UserId
            );

            foreach (var userNotification in userNotifications)
            {
                userNotification.ChangeState(state);
            }
        }
    }

    public virtual async Task DeleteUserNotificationAsync(Guid? tenantId, Guid userNotificationId)
    {
        using (CurrentTenant.Change(tenantId))
        {
            await _userNotificationRepository.DeleteAsync(userNotificationId);
        }
    }

    public virtual async Task DeleteAllUserNotificationsAsync(
        UserIdentifier user,
        UserNotificationState? state = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        using (CurrentTenant.Change(user.TenantId))
        {
            var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);

            await _userNotificationRepository.DeleteAsync(predicate);
        }
    }

    private Expression<Func<UserNotification, bool>> CreateNotificationFilterPredicate(
        UserIdentifier user,
        UserNotificationState? state = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var predicate = PredicateBuilder.New<UserNotification>();
        predicate = predicate.And(p => p.UserId == user.UserId);

        if (startDate.HasValue)
        {
            predicate = predicate.And(p => p.CreationTime >= startDate);
        }

        if (endDate.HasValue)
        {
            predicate = predicate.And(p => p.CreationTime <= endDate);
        }

        if (state.HasValue)
        {
            predicate = predicate.And(p => p.State == state);
        }

        return predicate;
    }

    public virtual async Task<List<UserNotificationWithNotification>>
        GetUserNotificationsWithNotificationsAsync(
            UserIdentifier user,
            UserNotificationState? state = null,
            int skipCount = 0,
            int maxResultCount = int.MaxValue,
            DateTime? startDate = null,
            DateTime? endDate = null)
    {
        using (CurrentTenant.Change(user.TenantId))
        {
            var userNotificationInfos = await _userNotificationRepository.GetQueryableAsync();
            var tenantNotificationInfos = await _tenantNotificationRepository.GetQueryableAsync();

            var query = from userNotificationInfo in userNotificationInfos
                        join tenantNotificationInfo in tenantNotificationInfos on userNotificationInfo
                            .TenantNotificationId equals tenantNotificationInfo.Id
                        where userNotificationInfo.UserId == user.UserId
                        orderby tenantNotificationInfo.CreationTime descending
                        select new
                        {
                            userNotificationInfo,
                            tenantNotificationInfo
                        };

            if (state.HasValue)
            {
                query = query.Where(x => x.userNotificationInfo.State == state.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(x => x.tenantNotificationInfo.CreationTime >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.tenantNotificationInfo.CreationTime <= endDate);
            }

            query = query.PageBy(skipCount, maxResultCount);

            var list = await AsyncExecuter.ToListAsync(query);

            return list.Select(
                a => new UserNotificationWithNotification(
                    a.userNotificationInfo,
                    a.tenantNotificationInfo
                )
            ).ToList();
        }
    }

    public virtual async Task<long> GetUserNotificationCountAsync(
        UserIdentifier user,
        UserNotificationState? state = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        using (CurrentTenant.Change(user.TenantId))
        {
            var predicate = CreateNotificationFilterPredicate(user, state, startDate, endDate);
            return await _userNotificationRepository.LongCountAsync(predicate);
        }
    }

    public virtual async Task<UserNotificationWithNotification>
        GetUserNotificationWithNotificationOrNullAsync(
            Guid? tenantId,
            Guid userNotificationId)
    {
        using (CurrentTenant.Change(tenantId))
        {
            var userNotificationInfos = await _userNotificationRepository.GetQueryableAsync();
            var tenantNotificationInfos = await _tenantNotificationRepository.GetQueryableAsync();

            var query = from userNotificationInfo in userNotificationInfos
                        join tenantNotificationInfo in tenantNotificationInfos on userNotificationInfo
                            .TenantNotificationId equals tenantNotificationInfo.Id
                        where userNotificationInfo.Id == userNotificationId
                        select new
                        {
                            userNotificationInfo,
                            tenantNotificationInfo
                        };

            var item = await AsyncExecuter.FirstOrDefaultAsync(query);
            if (item == null)
            {
                return null;
            }

            return new UserNotificationWithNotification(
                item.userNotificationInfo,
                item.tenantNotificationInfo
            );
        }
    }

    public virtual async Task InsertTenantNotificationAsync(TenantNotification tenantNotificationInfo)
    {
        using (CurrentTenant.Change(tenantNotificationInfo.TenantId))
        {
            await _tenantNotificationRepository.InsertAsync(tenantNotificationInfo);
        }
    }

    public virtual async Task DeleteNotificationAsync(Notification notification)
    {
        await _notificationRepository.DeleteAsync(notification);
    }

    public async Task<List<GetNotificationsCreatedByUserOutput>> GetNotificationsPublishedByUserAsync(
        UserIdentifier user,
        string notificationName,
        DateTime? startDate,
        DateTime? endDate)
    {
        using (CurrentTenant.Change(user.TenantId))
        {
            var notifications = await _notificationRepository.GetQueryableAsync();
            var queryForNotPublishedNotifications = notifications
                .Where(n => n.CreatorId == user.UserId && n.NotificationName == notificationName);

            if (startDate.HasValue)
            {
                queryForNotPublishedNotifications = queryForNotPublishedNotifications
                    .Where(x => x.CreationTime >= startDate);
            }

            if (endDate.HasValue)
            {
                queryForNotPublishedNotifications = queryForNotPublishedNotifications
                    .Where(x => x.CreationTime <= endDate);
            }

            var result = new List<GetNotificationsCreatedByUserOutput>();

            var unPublishedNotifications = await AsyncExecuter.ToListAsync(queryForNotPublishedNotifications
                .Select(x =>
                    new GetNotificationsCreatedByUserOutput()
                    {
                        Data = x.Data,
                        Severity = x.Severity,
                        NotificationName = x.NotificationName,
                        DataTypeName = x.DataTypeName,
                        IsPublished = false,
                        CreationTime = x.CreationTime
                    })
            );

            result.AddRange(unPublishedNotifications);
            ;
            var tenantNotifications = await _tenantNotificationRepository.GetQueryableAsync();
            var queryForPublishedNotifications = tenantNotifications
                .Where(n => n.CreatorId == user.UserId && n.NotificationName == notificationName);

            if (startDate.HasValue)
            {
                queryForPublishedNotifications = queryForPublishedNotifications
                    .Where(x => x.CreationTime >= startDate);
            }

            if (endDate.HasValue)
            {
                queryForPublishedNotifications = queryForPublishedNotifications
                    .Where(x => x.CreationTime <= endDate);
            }

            queryForPublishedNotifications = queryForPublishedNotifications
                .OrderByDescending(n => n.CreationTime);

            var publishedNotifications = await AsyncExecuter.ToListAsync(queryForPublishedNotifications
                .Select(x =>
                    new GetNotificationsCreatedByUserOutput()
                    {
                        Data = x.Data,
                        Severity = x.Severity,
                        NotificationName = x.NotificationName,
                        DataTypeName = x.DataTypeName,
                        IsPublished = true,
                        CreationTime = x.CreationTime
                    })
            );

            result.AddRange(publishedNotifications);
            return result;
        }
    }
}
