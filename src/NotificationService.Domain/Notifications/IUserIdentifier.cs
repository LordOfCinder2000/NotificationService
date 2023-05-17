using System;
using Volo.Abp.MultiTenancy;

namespace NotificationService.Notifications;

/// <summary>
/// Interface to get a user identifier.
/// </summary>
public interface IUserIdentifier : IMultiTenant
{
    /// <summary>
    /// Id of the user.
    /// </summary>
    Guid UserId { get; }
}