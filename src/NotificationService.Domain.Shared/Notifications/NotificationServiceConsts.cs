using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Notifications;
public static class NotificationServiceConsts
{
    /// <summary>
    /// Indicates all tenant ids for <see cref="TenantIds"/> property.
    /// Value: "0".
    /// </summary>
    public const string AllTenantIds = "0";

    /// <summary>
    /// Maximum length of <see cref="NotificationName"/> property.
    /// Value: 96.
    /// </summary>
    public const int MaxNotificationNameLength = 96;

    /// <summary>
    /// Maximum length of <see cref="Data"/> property.
    /// Value: 1048576 (1 MB).
    /// </summary>
    public const int MaxDataLength = 1024 * 1024;

    /// <summary>
    /// Maximum length of <see cref="DataTypeName"/> property.
    /// Value: 512.
    /// </summary>
    public const int MaxDataTypeNameLength = 512;

    /// <summary>
    /// Maximum length of <see cref="EntityTypeName"/> property.
    /// Value: 250.
    /// </summary>
    public const int MaxEntityTypeNameLength = 250;

    /// <summary>
    /// Maximum length of <see cref="EntityTypeAssemblyQualifiedName"/> property.
    /// Value: 512.
    /// </summary>
    public const int MaxEntityTypeAssemblyQualifiedNameLength = 512;

    /// <summary>
    /// Maximum length of <see cref="EntityId"/> property.
    /// Value: 96.
    /// </summary>
    public const int MaxEntityIdLength = 96;

    /// <summary>
    /// Maximum length of <see cref="UserIds"/> property.
    /// Value: 131072 (128 KB).
    /// </summary>
    public const int MaxUserIdsLength = 128 * 1024;

    /// <summary>
    /// Maximum length of <see cref="TenantIds"/> property.
    /// Value: 131072 (128 KB).
    /// </summary>
    public const int MaxTenantIdsLength = 128 * 1024;

    public const int MaxTargetNotifiersLength = 1024;

    /// <summary>
    /// Notification target list separation character.
    /// </summary>
    public const char NotificationTargetSeparator = ',';
}
