using System.Security.Claims;
using CqrsProject.Common.Consts;

namespace CqrsProject.Core.Identity;

public class CurrentIdentity : ICurrentIdentity
{
    private ClaimsPrincipal? _principal;

    public void SetCurrentIdentity(ClaimsPrincipal principal) => _principal = principal;

    public bool HasLocalIdentity()
    {
        return _principal?.Identities
            .Any(identity => identity.AuthenticationType == AuthenticationDefaults.LocalIdentityType)
            ?? false;
    }

    public Guid GetLocalIdentityId(){
        var id = _principal?.Identities
            .FirstOrDefault(identity => identity.AuthenticationType == AuthenticationDefaults.LocalIdentityType)
            ?.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (string.IsNullOrEmpty(id))
            throw new UnauthorizedAccessException();

        return Guid.Parse(id);
    }
}
