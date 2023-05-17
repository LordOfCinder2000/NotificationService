using System;
using Volo.Abp.Domain.Repositories;

namespace NotificationService.Notifications;
public interface INotificationRepository : IRepository<Notification, Guid>
{
}
