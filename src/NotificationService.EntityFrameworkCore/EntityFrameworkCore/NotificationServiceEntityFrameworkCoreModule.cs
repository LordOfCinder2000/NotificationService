using Microsoft.Extensions.DependencyInjection;
using NotificationService.Notifications;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace NotificationService.EntityFrameworkCore;

[DependsOn(
    typeof(NotificationServiceDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
[DependsOn(typeof(AbpIdentityEntityFrameworkCoreModule))]
[DependsOn(typeof(AbpSettingManagementEntityFrameworkCoreModule))]
    public class NotificationServiceEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<NotificationServiceDbContext>(options =>
        {
            /* Add custom repositories here. Example:
             * options.AddRepository<Question, EfCoreQuestionRepository>();
             */

            options.AddRepository<Notification, EfCoreNotificationRepository>();
            options.AddRepository<NotificationSubscription, EfCoreNotificationSubscriptionRepository>();
            options.AddRepository<UserNotification, EfCoreUserNotificationRepository>();
            options.AddRepository<TenantNotification, EfCoreTenantNotificationRepository>();
        });
    }
}
