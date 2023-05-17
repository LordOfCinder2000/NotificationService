using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace NotificationService.Notifications;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(NotificationServiceHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class NotificationServiceConsoleApiClientModule : AbpModule
{

}
