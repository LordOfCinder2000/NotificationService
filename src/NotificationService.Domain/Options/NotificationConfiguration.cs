using NotificationService.Notifications;
using Volo.Abp.Collections;
using Volo.Abp.DependencyInjection;

namespace NotificationService.Options;
public class NotificationConfiguration : INotificationConfiguration, ISingletonDependency
{
    public ITypeList<NotificationProvider> Providers { get; private set; }

    public ITypeList<INotificationDistributer> Distributers { get; private set; }

    public ITypeList<IRealTimeNotifier> Notifiers { get; private set; }

    public NotificationConfiguration()
    {
        Providers = new TypeList<NotificationProvider>();
        Distributers = new TypeList<INotificationDistributer>();
        Notifiers = new TypeList<IRealTimeNotifier>();
    }
}
