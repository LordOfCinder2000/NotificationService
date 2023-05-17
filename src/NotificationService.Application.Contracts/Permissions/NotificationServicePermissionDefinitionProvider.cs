using NotificationService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace NotificationService.Permissions;

public class NotificationServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(NotificationServicePermissions.GroupName, L("Permission:NotificationService"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<NotificationServiceResource>(name);
    }
}
