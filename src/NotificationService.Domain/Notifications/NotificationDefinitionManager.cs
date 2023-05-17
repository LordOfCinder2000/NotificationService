using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Security.Claims;
using Volo.Abp.SimpleStateChecking;
using Volo.Abp.Users;
using Volo.Abp.Identity;
using Microsoft.AspNetCore.Identity;
using NotificationService.Options;
using Microsoft.Extensions.Options;

namespace NotificationService.Notifications;

/// <summary>
/// Implements <see cref="INotificationDefinitionManager"/>.
/// </summary>
internal class NotificationDefinitionManager : DomainService, INotificationDefinitionManager, ISingletonDependency
{
    private readonly NotificationServiceOptions _notificationServiceOptions;
    private readonly ISimpleStateCheckerManager<NotificationDefinition> _stateCheckerManager;
    private readonly IUserClaimsPrincipalFactory<IdentityUser> _userClaimsPrincipalFactory;
    private readonly IdentityUserManager _identityUserManager;
    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
    private readonly IDictionary<string, NotificationDefinition> _notificationDefinitions;

    public NotificationDefinitionManager(
        IOptions<NotificationServiceOptions> notificationServiceOptions,
        ISimpleStateCheckerManager<NotificationDefinition> stateCheckerManager,
        IUserClaimsPrincipalFactory<IdentityUser> userClaimsPrincipalFactory,
        IdentityUserManager identityUserManager,
        ICurrentPrincipalAccessor currentPrincipalAccessor
        )
    {
        _notificationServiceOptions = notificationServiceOptions.Value;
        _stateCheckerManager = stateCheckerManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _identityUserManager = identityUserManager;
        _currentPrincipalAccessor = currentPrincipalAccessor;
        _notificationDefinitions = new Dictionary<string, NotificationDefinition>();
    }

    public void Initialize()
    {
        var context = new NotificationDefinitionContext(this);
        foreach (var providerType in _notificationServiceOptions.Configuration.Providers)
        {
            var provider = (NotificationProvider)LazyServiceProvider.LazyGetRequiredService(providerType);
            provider.SetNotifications(context);
        }
    }

    public void Add(NotificationDefinition notificationDefinition)
    {
        if (_notificationDefinitions.ContainsKey(notificationDefinition.Name))
        {
            throw new AbpInitializationException("There is already a notification definition with given name: " + notificationDefinition.Name + ". Notification names must be unique!");
        }

        _notificationDefinitions[notificationDefinition.Name] = notificationDefinition;
    }

    public NotificationDefinition Get(string name)
    {
        var definition = GetOrNull(name);
        if (definition == null)
        {
            throw new AbpException("There is no notification definition with given name: " + name);
        }

        return definition;
    }

    public NotificationDefinition GetOrNull(string name)
    {
        return _notificationDefinitions.GetOrDefault(name);
    }

    public void Remove(string name)
    {
        _notificationDefinitions.Remove(name);
    }

    public IReadOnlyList<NotificationDefinition> GetAll()
    {
        return _notificationDefinitions.Values.ToImmutableList();
    }

    public async Task<bool> IsAvailableAsync(string name, UserIdentifier user)
    {
        var notificationDefinition = GetOrNull(name);
        if (notificationDefinition == null)
        {
            return true;
        }


        if (!notificationDefinition.StateCheckers.IsNullOrEmpty())
        {
            var identityUser = await _identityUserManager.FindByIdAsync(user.UserId.ToString());

            if (identityUser == null)
            {
                return false;
            }

            var claimsPrincipal = await _userClaimsPrincipalFactory.CreateAsync(identityUser);
            using (_currentPrincipalAccessor.Change(claimsPrincipal))
            {
                using (CurrentTenant.Change(user.TenantId))
                {
                    return await _stateCheckerManager.IsEnabledAsync(notificationDefinition);
                }
            }
        }

        return true;
    }

    public async Task<IReadOnlyList<NotificationDefinition>> GetAllAvailableAsync(UserIdentifier user)
    {
        var availableDefinitions = new List<NotificationDefinition>();

        foreach (var notificationDefinition in GetAll())
        {
            if (!notificationDefinition.StateCheckers.IsNullOrEmpty())
            {
                var identityUser = await _identityUserManager.FindByIdAsync(user.UserId.ToString());

                if (identityUser == null)
                {
                    continue;
                }

                var claimsPrincipal = await _userClaimsPrincipalFactory.CreateAsync(identityUser);
                using (_currentPrincipalAccessor.Change(claimsPrincipal))
                {
                    using (CurrentTenant.Change(user.TenantId))
                    {
                        if(!await _stateCheckerManager.IsEnabledAsync(notificationDefinition))
                        {
                            continue;
                        }
                    }
                }
            }

            availableDefinitions.Add(notificationDefinition);
        }

        return availableDefinitions.ToImmutableList();
    }
}