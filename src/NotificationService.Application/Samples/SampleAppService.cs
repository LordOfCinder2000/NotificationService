using NotificationService.Notifications;
using System;
using System.Threading.Tasks;

namespace NotificationService.Samples;

public class SampleAppService : NotificationServiceAppService, ISampleAppService
{
    private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
    private readonly INotificationPublisher _notificationPublisher;

    public SampleAppService(
        INotificationSubscriptionManager notificationSubscriptionManager,
        INotificationPublisher notificationPublisher)
    {
        _notificationSubscriptionManager = notificationSubscriptionManager;
        _notificationPublisher = notificationPublisher;
    }

    public virtual async Task SubscriberAsync()
    {
        await _notificationSubscriptionManager.SubscribeAsync(
             new UserIdentifier(null, Guid.Parse("5f1ab144-07ca-927f-ace2-3a0b3abb53d7")),
             "SentFriendshipRequest");
    }

    public virtual async Task PublisherAsync()
    {
        await _notificationPublisher.PublishAsync
            ("SentFriendshipRequest",
            new MessageNotificationData("SentFriendshipRequest message!"),
            userIds: new[] { new UserIdentifier(null, Guid.Parse("5f1ab144-07ca-927f-ace2-3a0b3abb53d7")) });
    }
}
