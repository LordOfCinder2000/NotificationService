using System;
using Volo.Abp.Domain.Repositories;

namespace NotificationService.Notifications;
public interface INotificationSubscriptionRepository : IRepository<NotificationSubscription, Guid>
{
}
