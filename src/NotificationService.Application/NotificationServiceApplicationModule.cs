using Microsoft.Extensions.DependencyInjection;
using NotificationService.Options;
using NotificationService.SignalR;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;

namespace NotificationService.Notifications;

[DependsOn(
    typeof(NotificationServiceDomainModule),
    typeof(NotificationServiceApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
[DependsOn(typeof(AbpIdentityApplicationModule))]
[DependsOn(typeof(AbpSettingManagementApplicationModule))]
public class NotificationServiceApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<NotificationServiceApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<NotificationServiceApplicationModule>(validate: true);
        });

        Configure<NotificationServiceOptions>(options =>
        {
            options.Configuration.Notifiers.Add<SignalRRealTimeNotifier>();
        });
    }
}
