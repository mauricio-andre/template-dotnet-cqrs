using CqrsProject.Core.Identity.Interfaces;
using MediatR;
using CqrsProject.Core.Identity.UseCases.CreateUserRole;
using CqrsProject.Core.UserTenants.UseCases.CreateUserTenant;
using CqrsProject.Core.Identity.UseCases.CreateRoleClaim;
using CqrsProject.Core.Identity.Consts;

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
            && !_currentIdentity.HasLocalPermission(IdentityPermissionClaimDefaults.ManageSelf))
        {
            throw new UnauthorizedAccessException();
        }

        return Task.CompletedTask;
    }
}
