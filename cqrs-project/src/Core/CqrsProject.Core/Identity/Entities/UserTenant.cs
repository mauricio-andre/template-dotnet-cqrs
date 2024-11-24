using CqrsProject.Core.Tenants;

namespace CqrsProject.Core.Identity;

public class UserTenant
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }

    public User? User { get; set; }
    public Tenant? Tenant { get; set; }
}
