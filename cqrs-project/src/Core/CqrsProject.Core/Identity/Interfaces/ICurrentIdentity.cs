using System.Security.Claims;

namespace CqrsProject.Core.Identity;

public interface ICurrentIdentity
{
    public void SetCurrentIdentity(ClaimsPrincipal principal);
    public bool HasLocalIdentity();
    public Guid GetLocalIdentityId();
}
