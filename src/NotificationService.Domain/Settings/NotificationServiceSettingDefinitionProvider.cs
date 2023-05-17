using NotificationService.Localization;
using System;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace NotificationService.Settings;

public class NotificationServiceSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        /* Define module settings here.
         * Use names from NotificationServiceSettings class.
         */
        context.Add(
                new SettingDefinition(
                    NotificationServiceSettings.Notification.ReceiveNotifications,
                    "true",
                    L("ReceiveNotifications"),
                    isVisibleToClients: true
                )
            );
    }

    private static LocalizableString L(string name)
    {
        return new LocalizableString(typeof(NotificationServiceResource), name);
    }
}
