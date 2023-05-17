using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NotificationService.Options;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Json;
using Volo.Abp.Uow;

namespace NotificationService.Notifications;

/// <summary>
/// Implements <see cref="INotificationPublisher"/>.
/// </summary>
public class NotificationPublisher : DomainService, INotificationPublisher, ITransientDependency
{
    public const int MaxUserCountToDirectlyDistributeANotification = 5;

    /// <summary>
    /// Indicates all tenants.
    /// </summary>
    public static int[] AllTenants => new[] { NotificationServiceConsts.AllTenantIds.To<int>() };

    private readonly INotificationStore _store;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly INotificationDistributer _notificationDistributer;
    private readonly NotificationServiceOptions _notificationServiceOptions;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IJsonSerializer _jsonSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationPublisher"/> class.
    /// </summary>
    public NotificationPublisher(
        INotificationStore store,
        IBackgroundJobManager backgroundJobManager,
        INotificationDistributer notificationDistributer,
        IOptions<NotificationServiceOptions> notificationServiceOptions,
        IUnitOfWorkManager unitOfWorkManager,
        IJsonSerializer jsonSerializer)
    {
        _store = store;
        _backgroundJobManager = backgroundJobManager;
        _notificationDistributer = notificationDistributer;
        _notificationServiceOptions = notificationServiceOptions.Value;
        _unitOfWorkManager = unitOfWorkManager;
        _jsonSerializer = jsonSerializer;
    }

    public virtual async Task PublishAsync(
        string notificationName,
        NotificationData data = null,
        EntityIdentifier entityIdentifier = null,
        NotificationSeverity severity = NotificationSeverity.Info,
        UserIdentifier[] userIds = null,
        UserIdentifier[] excludedUserIds = null,
        Guid?[] tenantIds = null,
        Type[] targetNotifiers = null)
    {
        if (notificationName.IsNullOrEmpty())
        {
            throw new ArgumentException("NotificationName can not be null or whitespace!", nameof(notificationName));
        }

        if (!tenantIds.IsNullOrEmpty() && !userIds.IsNullOrEmpty())
        {
            throw new ArgumentException("tenantIds can be set only if userIds is not set!", nameof(tenantIds));
        }

        if (tenantIds.IsNullOrEmpty() && userIds.IsNullOrEmpty())
        {
            tenantIds = new[] { CurrentTenant.Id };
        }

        var notificationInfo = new Notification(
            GuidGenerator.Create(),
            notificationName,
            entityIdentifier?.Type.FullName,
            entityIdentifier?.Type.AssemblyQualifiedName,
            entityIdentifier?.Id == null ? null : _jsonSerializer.Serialize(entityIdentifier.Id),
            userIds.IsNullOrEmpty() ? null : userIds.Select(uid => uid.ToUserIdentifierString()).JoinAsString(","),
            excludedUserIds.IsNullOrEmpty() ? null : excludedUserIds.Select(uid => uid.ToUserIdentifierString()).JoinAsString(","),
            GetTenantIdsAsStr(tenantIds),
            data == null ? null : _jsonSerializer.Serialize(data),
            data?.GetType().AssemblyQualifiedName,
            severity
            );

        SetTargetNotifiers(notificationInfo, targetNotifiers);

        await _store.InsertNotificationAsync(notificationInfo);

        await _unitOfWorkManager.Current.SaveChangesAsync(); //To get Id of the notification

        if (userIds != null && userIds.Length <= MaxUserCountToDirectlyDistributeANotification)
        {
            //We can directly distribute the notification since there are not much receivers
            await _notificationDistributer.DistributeAsync(notificationInfo.Id);
        }
        else
        {
            //We enqueue a background job since distributing may get a long time
            await _backgroundJobManager.EnqueueAsync(
                new NotificationDistributionJobArgs(
                    notificationInfo.Id
                )
            );
        }
    }

    protected virtual void SetTargetNotifiers(Notification notificationInfo, Type[] targetNotifiers)
    {
        if (targetNotifiers == null)
        {
            return;
        }

        var allNotificationNotifiers = _notificationServiceOptions.Configuration.Notifiers.Select(notifier => notifier.FullName).ToList();

        foreach (var targetNotifier in targetNotifiers)
        {
            if (!allNotificationNotifiers.Contains(targetNotifier.FullName))
            {
                throw new ApplicationException("Given target notifier is not registered before: " + targetNotifier.FullName + " You must register it to the INotificationConfiguration.Notifiers!");
            }
        }

        notificationInfo.SetTargetNotifiers(targetNotifiers.Select(n => n.FullName).ToList());
    }

    /// <summary>
    /// Gets the string for <see cref="Notification.TenantIds"/>.
    /// </summary>
    /// <param name="tenantIds"></param>
    /// <seealso cref="DefaultNotificationDistributer.GetTenantIds"/>
    private static string GetTenantIdsAsStr(Guid?[] tenantIds)
    {
        if (tenantIds.IsNullOrEmpty())
        {
            return null;
        }

        return tenantIds
            .Select(tenantId => tenantId == null ? "null" : tenantId.ToString())
            .JoinAsString(",");
    }
}
