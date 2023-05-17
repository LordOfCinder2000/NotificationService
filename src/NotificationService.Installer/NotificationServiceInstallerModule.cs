using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace NotificationService.Notifications;

[DependsOn(
    typeof(AbpVirtualFileSystemModule)
    )]
public class NotificationServiceInstallerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<NotificationServiceInstallerModule>();
        });
    }
}
