using NotificationService.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace NotificationService.Notifications;
internal class EfCoreNotificationRepository : EfCoreRepository<NotificationServiceDbContext, Notification, Guid>,
        INotificationRepository
{
    public EfCoreNotificationRepository(IDbContextProvider<NotificationServiceDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}
