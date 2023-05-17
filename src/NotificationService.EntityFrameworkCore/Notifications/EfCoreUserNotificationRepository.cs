using NotificationService.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace NotificationService.Notifications;
internal class EfCoreUserNotificationRepository : EfCoreRepository<NotificationServiceDbContext, UserNotification, Guid>,
        IUserNotificationRepository
{
    public EfCoreUserNotificationRepository(IDbContextProvider<NotificationServiceDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}
