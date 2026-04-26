using System.Security.Claims;

namespace CqrsProject.Core.Identity.Interfaces;

public interface ICurrentIdentity
{
    public void SetCurrentIdentity(ClaimsPrincipal? principal);
    public ClaimsPrincipal? GetCurrentIdentity();
    public bool HasLocalIdentity();
    public bool HasLocalPermission(string permissionName);
    public Guid GetLocalIdentityId();
    public IEnumerable<string> GetRoles();
    public IEnumerable<Guid> GetTenants();
}
