using NotificationService.Localization;
using Volo.Abp.Application.Services;

namespace NotificationService.Notifications;

public abstract class NotificationServiceAppService : ApplicationService
{
    protected NotificationServiceAppService()
    {
        LocalizationResource = typeof(NotificationServiceResource);
        ObjectMapperContext = typeof(NotificationServiceApplicationModule);
    }
}
