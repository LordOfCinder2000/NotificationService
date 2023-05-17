using System;
using Volo.Abp.Domain.Repositories;

namespace NotificationService.Notifications;
public interface ITenantNotificationRepository : IRepository<TenantNotification, Guid>
{
}
