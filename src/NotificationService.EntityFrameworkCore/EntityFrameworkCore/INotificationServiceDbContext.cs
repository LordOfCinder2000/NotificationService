using Microsoft.EntityFrameworkCore;
using NotificationService.Notifications;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace NotificationService.EntityFrameworkCore;

[ConnectionStringName(NotificationServiceDbProperties.ConnectionStringName)]
public interface INotificationServiceDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
    DbSet<Notification> Notifications { get; }
    DbSet<NotificationSubscription> NotificationSubscriptions { get; }
    DbSet<UserNotification> UserNotifications { get; }
    DbSet<TenantNotification> TenantNotifications { get; }
}
