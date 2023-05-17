using NotificationService.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace NotificationService.Notifications;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(NotificationServiceEntityFrameworkCoreTestModule)
    )]
public class NotificationServiceDomainTestModule : AbpModule
{

}
