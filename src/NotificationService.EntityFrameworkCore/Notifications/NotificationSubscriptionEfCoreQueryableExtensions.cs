using System.Linq;

namespace NotificationService.Notifications;
public static class NotificationSubscriptionEfCoreQueryableExtensions
{
    public static IQueryable<NotificationSubscription> IncludeDetails(this IQueryable<NotificationSubscription> queryable, bool include = true)
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
