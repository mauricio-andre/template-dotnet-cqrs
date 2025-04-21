using System.Security.Claims;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Identity.Interfaces;

namespace CqrsProject.Core.Identity.Services;

public class CurrentIdentity : ICurrentIdentity
{
    private ClaimsPrincipal? _principal;

    public void SetCurrentIdentity(ClaimsPrincipal? principal) => _principal = principal;

    public bool HasLocalIdentity()
    {
        return _principal?.Identities
            .Any(identity => identity.AuthenticationType == AuthenticationDefaults.LocalIdentityType)
            ?? false;
    }

    public Guid GetLocalIdentityId()
    {
        var id = _principal?.Identities
            .FirstOrDefault(identity => identity.AuthenticationType == AuthenticationDefaults.LocalIdentityType)
            ?.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (string.IsNullOrEmpty(id))
            throw new UnauthorizedAccessException();

        return Guid.Parse(id);
    }

    public bool HasLocalPermission(string permissionName)
    {
        return _principal?.Identities
            .FirstOrDefault(identity => identity.AuthenticationType == AuthenticationDefaults.LocalIdentityType)
            ?.Claims.Any(claim => claim.Type == AuthorizationPermissionClaims.ClaimType
                && claim.Value == permissionName)
            ?? false;
    }
}
