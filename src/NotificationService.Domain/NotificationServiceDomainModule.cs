using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Domain;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;

namespace NotificationService.Notifications;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(NotificationServiceDomainSharedModule)
)]
[DependsOn(typeof(AbpIdentityDomainModule))]
[DependsOn(typeof(AbpSettingManagementDomainModule))]
public class NotificationServiceDomainModule : AbpModule
{
    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        context.ServiceProvider.GetRequiredService<NotificationDefinitionManager>().Initialize();
    }
}
