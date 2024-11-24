using CqrsProject.Core.Tenants;
using Microsoft.AspNetCore.Identity;

namespace CqrsProject.Core.Identity;

public class User : IdentityUser<Guid>
{
    public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? LastModificationTime { get; set; }
    public bool IsDeleted { get; set; } = false;

    public ICollection<UserTenant> UserTenantList { get; set; } = new List<UserTenant>();
    public ICollection<Tenant> TenantList { get; set; } = new List<Tenant>();
}
