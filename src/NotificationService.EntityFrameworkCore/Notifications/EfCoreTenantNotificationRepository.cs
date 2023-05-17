using NotificationService.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace NotificationService.Notifications;
internal class EfCoreTenantNotificationRepository : EfCoreRepository<NotificationServiceDbContext, TenantNotification, Guid>,
        ITenantNotificationRepository
{
    public EfCoreTenantNotificationRepository(IDbContextProvider<NotificationServiceDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}
