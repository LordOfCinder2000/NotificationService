using System.Linq;

namespace NotificationService.Notifications;
public static class NotificationEfCoreQueryableExtensions
{
    public static IQueryable<Notification> IncludeDetails(this IQueryable<Notification> queryable, bool include = true)
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
