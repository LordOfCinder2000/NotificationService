using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectMapping;

namespace NotificationService.Notifications;

/// <summary>
/// Implements  <see cref="IUserNotificationManager"/>.
/// </summary>
public class UserNotificationManager : DomainService, IUserNotificationManager, ISingletonDependency
{
    private readonly INotificationStore _store;
    private readonly IObjectMapper _objectMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserNotificationManager"/> class.
    /// </summary>
    public UserNotificationManager(
        INotificationStore store,
        IObjectMapper objectMapper)
    {
        _store = store;
        _objectMapper = objectMapper;
    }

    public async Task<List<UserNotificationInfo>> GetUserNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, int skipCount = 0, int maxResultCount = int.MaxValue, DateTime? startDate = null, DateTime? endDate = null)
    {
        var userNotifications = await _store.GetUserNotificationsWithNotificationsAsync(user, state, skipCount, maxResultCount, startDate, endDate);

        return _objectMapper.Map<List<UserNotificationWithNotification>, List<UserNotificationInfo>>(userNotifications);
        //return userNotifications
        //    .Select(un => un.UserNotification)
        //    //.Select(un => un.ToUserNotification())
        //    .ToList();
    }

    public async Task<long> GetUserNotificationCountAsync(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        return await _store.GetUserNotificationCountAsync(user, state, startDate, endDate);
    }

    public async Task<UserNotificationInfo> GetUserNotificationAsync(Guid? tenantId, Guid userNotificationId)
    {
        var userNotification = await _store.GetUserNotificationWithNotificationOrNullAsync(tenantId, userNotificationId);
        if (userNotification == null)
        {
            return null;
        }

        return _objectMapper.Map<UserNotificationWithNotification, UserNotificationInfo>(userNotification);
        //return userNotification.UserNotification;
        //.ToUserNotification();
    }

    public async Task UpdateUserNotificationStateAsync(Guid? tenantId, Guid userNotificationId, UserNotificationState state)
    {
        await _store.UpdateUserNotificationStateAsync(tenantId, userNotificationId, state);
    }

    public async Task UpdateAllUserNotificationStatesAsync(UserIdentifier user, UserNotificationState state)
    {
        await _store.UpdateAllUserNotificationStatesAsync(user, state);
    }

    public async Task DeleteUserNotificationAsync(Guid? tenantId, Guid userNotificationId)
    {
        await _store.DeleteUserNotificationAsync(tenantId, userNotificationId);
    }

    public async Task DeleteAllUserNotificationsAsync(UserIdentifier user, UserNotificationState? state = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        await _store.DeleteAllUserNotificationsAsync(user, state, startDate, endDate);
    }
}