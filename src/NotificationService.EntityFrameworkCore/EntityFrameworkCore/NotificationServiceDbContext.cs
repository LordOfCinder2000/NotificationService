using Microsoft.EntityFrameworkCore;
using NotificationService.Notifications;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace NotificationService.EntityFrameworkCore;

[ConnectionStringName(NotificationServiceDbProperties.ConnectionStringName)]
public class NotificationServiceDbContext : AbpDbContext<NotificationServiceDbContext>, INotificationServiceDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }
    public DbSet<UserNotification> UserNotifications { get; set; }
    public DbSet<TenantNotification> TenantNotifications { get; set; }


    public NotificationServiceDbContext(DbContextOptions<NotificationServiceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureNotificationService();
        builder.ConfigureIdentity();
        builder.ConfigureSettingManagement();
    }
}
