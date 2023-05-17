using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NotificationService.Notifications;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace NotificationService.SignalR;

/// <summary>
/// Implements <see cref="IRealTimeNotifier"/> to send notifications via SignalR.
/// </summary>
public class SignalRRealTimeNotifier : IRealTimeNotifier, ITransientDependency
{
    public bool UseOnlyIfRequestedAsTarget => false;

    /// <summary>
    /// Reference to the logger.
    /// </summary>
    public ILogger<SignalRRealTimeNotifier> Logger { get; set; }

    private readonly IOnlineClientManager _onlineClientManager;

    private readonly IHubContext<OnlineClientHub> _hubContext;
    private readonly IObjectMapper _objectMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalRRealTimeNotifier"/> class.
    /// </summary>
    public SignalRRealTimeNotifier(
        IOnlineClientManager onlineClientManager,
        IHubContext<OnlineClientHub> hubContext,
        IObjectMapper objectMapper)
    {
        _onlineClientManager = onlineClientManager;
        _hubContext = hubContext;
        _objectMapper = objectMapper;
        Logger = NullLogger<SignalRRealTimeNotifier>.Instance;
    }

    /// <inheritdoc/>
    public async Task SendNotificationsAsync(UserNotificationInfo[] userNotificationInfos)
    {
        foreach (var userNotificationInfo in userNotificationInfos)
        {
            try
            {
                var onlineClients = await _onlineClientManager.GetAllByUserIdAsync(userNotificationInfo);
                foreach (var onlineClient in onlineClients)
                {
                    var signalRClient = _hubContext.Clients.Client(onlineClient.ConnectionId);
                    if (signalRClient == null)
                    {
                        Logger.LogDebug("Can not get user " + userNotificationInfo.ToUserIdentifier() + " with connectionId " + onlineClient.ConnectionId + " from SignalR hub!");
                        continue;
                    }

#pragma warning disable CS0618 // Type or member is obsolete, this line will be removed once the EntityType property is removed
                    userNotificationInfo.Notification.EntityType = null; // Serialization of System.Type causes SignalR to disconnect. See https://github.com/aspnetboilerplate/aspnetboilerplate/issues/5230
#pragma warning restore CS0618 // Type or member is obsolete, this line will be removed once the EntityType property is removed
                    await signalRClient.SendAsync("getNotification", userNotificationInfo);
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Could not send notification to user: " + userNotificationInfo.ToUserIdentifier());
                Logger.LogWarning(ex.ToString(), ex);
            }
        }
    }
}
