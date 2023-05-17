namespace NotificationService.Options;
public class NotificationServiceOptions
{
    public NotificationConfiguration Configuration { get; }

    public NotificationServiceOptions()
    {
        Configuration = new NotificationConfiguration();
    }
}
