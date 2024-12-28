using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Tenants.Entities;

namespace CqrsProject.Core.UserTenants.Entities;

public class UserTenant
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }

    public User? User { get; set; }
    public Tenant? Tenant { get; set; }
}
