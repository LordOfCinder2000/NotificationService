using AutoMapper;
using NotificationService.ObjectMappers;
using Volo.Abp.AutoMapper;

namespace NotificationService.Notifications;

public class NotificationServiceApplicationAutoMapperProfile : Profile
{
    public NotificationServiceApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<TenantNotification, TenantNotificationInfo>(MemberList.None);
        CreateMap<NotificationSubscription, NotificationSubscriptionInfo>(MemberList.None);
        CreateMap<UserNotificationWithNotification, UserNotificationInfo>(MemberList.None);
    }
}
