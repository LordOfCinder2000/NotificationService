using System;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Timing;

namespace NotificationService.SignalR;

/// <summary>
/// Implements <see cref="IOnlineClient"/>.
/// </summary>
[Serializable]
public class OnlineClient : ExtensibleObject, IOnlineClient
{
    /// <summary>
    /// Unique connection Id for this client.
    /// </summary>
    public string ConnectionId { get; set; }

    /// <summary>
    /// IP address of this client.
    /// </summary>
    public string IpAddress { get; set; }

    /// <summary>
    /// Tenant Id.
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// User Id.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Connection establishment time for this client.
    /// </summary>
    public DateTime ConnectTime { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlineClient"/> class.
    /// </summary>
    public OnlineClient(IClock clock)
    {
        ConnectTime = clock.Now;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlineClient"/> class.
    /// </summary>
    /// <param name="connectionId">The connection identifier.</param>
    /// <param name="ipAddress">The ip address.</param>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="userId">The user identifier.</param>
    public OnlineClient(string connectionId, string ipAddress, Guid? tenantId, Guid? userId, IClock clock)
        : this(clock)
    {
        ConnectionId = connectionId;
        IpAddress = ipAddress;
        TenantId = tenantId;
        UserId = userId;
    }
}