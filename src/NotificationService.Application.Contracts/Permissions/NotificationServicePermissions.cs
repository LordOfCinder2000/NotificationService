using Volo.Abp.Reflection;

namespace NotificationService.Permissions;

public class NotificationServicePermissions
{
    public const string GroupName = "NotificationService";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(NotificationServicePermissions));
    }
}
