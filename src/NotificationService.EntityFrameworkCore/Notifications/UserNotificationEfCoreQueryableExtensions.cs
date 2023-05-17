using System.Linq;

namespace NotificationService.Notifications;
public static class UserNotificationEfCoreQueryableExtensions
{
    public static IQueryable<UserNotification> IncludeDetails(this IQueryable<UserNotification> queryable, bool include = true)
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
