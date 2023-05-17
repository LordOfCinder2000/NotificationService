using Volo.Abp.Modularity;

namespace NotificationService.Notifications;

[DependsOn(
    typeof(NotificationServiceApplicationModule),
    typeof(NotificationServiceDomainTestModule)
    )]
public class NotificationServiceApplicationTestModule : AbpModule
{

}
