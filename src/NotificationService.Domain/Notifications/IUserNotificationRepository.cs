using System;
using Volo.Abp.Domain.Repositories;

namespace NotificationService.Notifications;
public interface IUserNotificationRepository : IRepository<UserNotification, Guid>
{
}
