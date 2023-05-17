using Volo.Abp;
using Volo.Abp.MongoDB;

namespace NotificationService.MongoDB;

public static class NotificationServiceMongoDbContextExtensions
{
    public static void ConfigureNotificationService(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
