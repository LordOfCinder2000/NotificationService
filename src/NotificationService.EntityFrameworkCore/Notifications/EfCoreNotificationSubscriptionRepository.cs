using NotificationService.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace NotificationService.Notifications;
internal class EfCoreNotificationSubscriptionRepository : EfCoreRepository<NotificationServiceDbContext, NotificationSubscription, Guid>,
        INotificationSubscriptionRepository
{
    public EfCoreNotificationSubscriptionRepository(IDbContextProvider<NotificationServiceDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}
