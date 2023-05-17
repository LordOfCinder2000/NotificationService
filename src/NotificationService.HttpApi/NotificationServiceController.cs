using NotificationService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace NotificationService.Notifications;

public abstract class NotificationServiceController : AbpControllerBase
{
    protected NotificationServiceController()
    {
        LocalizationResource = typeof(NotificationServiceResource);
    }
}
