using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace NotificationService.EntityFrameworkCore;

public class NotificationServiceHttpApiHostMigrationsDbContext : AbpDbContext<NotificationServiceHttpApiHostMigrationsDbContext>
{
    public NotificationServiceHttpApiHostMigrationsDbContext(DbContextOptions<NotificationServiceHttpApiHostMigrationsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureNotificationService();
        modelBuilder.ConfigureIdentity();
        //modelBuilder.ConfigureSettingManagement();
    }
}
