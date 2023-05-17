using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using Volo.Abp.SettingManagement.MongoDB;
using Volo.Abp.Identity.MongoDB;
using NotificationService.Notifications;

namespace NotificationService.MongoDB;

[DependsOn(
    typeof(NotificationServiceDomainModule),
    typeof(AbpMongoDbModule)
    )]
[DependsOn(typeof(AbpSettingManagementMongoDbModule))]
    [DependsOn(typeof(AbpIdentityMongoDbModule))]
    public class NotificationServiceMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<NotificationServiceMongoDbContext>(options =>
        {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, MongoQuestionRepository>();
                 */
        });
    }
}
