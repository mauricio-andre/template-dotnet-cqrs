using CqrsProject.Common.Consts;
using CqrsProject.Core.Identity.Events;
using CqrsProject.Core.Identity.Interfaces;
using CqrsProject.Core.UserTenants.Events;
using MediatR;

namespace CqrsProject.Core.Identity.Rules;

public class ShallNotAllowUnauthorizedUserManageSelfRule
    : INotificationHandler<CreateUserTenantEvent>,
    INotificationHandler<CreateUserRoleEvent>,
    INotificationHandler<CreateRoleClaimForYourselfEvent>
{
    private readonly ICurrentIdentity _currentIdentity;

    public ShallNotAllowUnauthorizedUserManageSelfRule(ICurrentIdentity currentIdentity)
    {
        _currentIdentity = currentIdentity;
    }

    public Task Handle(CreateUserTenantEvent notification, CancellationToken cancellationToken)
    {
        return HandleRule(notification.UserId);
    }

    public Task Handle(CreateUserRoleEvent notification, CancellationToken cancellationToken)
    {
        return HandleRule(notification.UserId);
    }

    public Task Handle(CreateRoleClaimForYourselfEvent notification, CancellationToken cancellationToken)
    {
        return HandleRule(notification.UserId);
    }

    public Task HandleRule(Guid userId)
    {
        if (userId == _currentIdentity.GetLocalIdentityId()
            && !_currentIdentity.HasLocalPermission(AuthorizationPermissionClaims.ManageSelf))
        {
            throw new UnauthorizedAccessException();
        }

        return Task.CompletedTask;
    }
}
