using CqrsProject.Common.Consts;
using CqrsProject.Core.UserTenants.Events;
using CqrsProject.Core.Identity.Interfaces;
using MediatR;

namespace CqrsProject.Core.Identity.Rules;

public class ShallNotAllowUnauthorizedUserManageSelfRule: INotificationHandler<CreateUserTenantEvent>
{
    private readonly ICurrentIdentity _currentIdentity;

    public ShallNotAllowUnauthorizedUserManageSelfRule(ICurrentIdentity currentIdentity)
    {
        _currentIdentity = currentIdentity;
    }

    public async Task Handle(CreateUserTenantEvent notification, CancellationToken cancellationToken)
    {
        if (notification.UserId == _currentIdentity.GetLocalIdentityId()
            && !_currentIdentity.HasLocalPermission(AuthorizationPermissionClaims.ManageSelf))
        {
            throw new UnauthorizedAccessException();
        }
    }
}
