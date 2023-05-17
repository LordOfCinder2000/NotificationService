using System.Linq;

namespace NotificationService.Notifications;
public static class TenantNotificationEfCoreQueryableExtensions
{
    public static IQueryable<TenantNotification> IncludeDetails(this IQueryable<TenantNotification> queryable, bool include = true)
    {
        if (!include)
        {
            return queryable;
        }

        return queryable
            // .Include(x => x.xxx) // TODO: AbpHelper generated
            ;
    }
}
