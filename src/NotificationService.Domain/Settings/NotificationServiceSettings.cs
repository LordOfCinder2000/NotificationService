namespace NotificationService.Settings;

public static class NotificationServiceSettings
{
    public const string GroupName = "NotificationService";

    /* Add constants for setting names. Example:
     * public const string MySettingName = GroupName + ".MySettingName";
     */
    public static class Notification
    {
        private const string NotificationsGroupName = GroupName + ".Notifications";

        public const string ReceiveNotifications = NotificationsGroupName + ".ReceiveNotifications";
    }
}
