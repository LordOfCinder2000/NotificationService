using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Data;

namespace NotificationService.SignalR;

    /// <summary>
    /// Represents an online client connected to the application.
    /// </summary>
    public interface IOnlineClient : IHasExtraProperties
{
        /// <summary>
        /// Unique connection Id for this client.
        /// </summary>
        string ConnectionId { get; }

        /// <summary>
        /// IP address of this client.
        /// </summary>
        string IpAddress { get; }

        /// <summary>
        /// Tenant Id.
        /// </summary>
        Guid? TenantId { get; }

        /// <summary>
        /// User Id.
        /// </summary>
        Guid? UserId { get; }

        /// <summary>
        /// Connection establishment time for this client.
        /// </summary>
        DateTime ConnectTime { get; }
    }