using CqrsProject.Core.Identity;

namespace CqrsProject.Core.Tenants;

public static class TenantConstrains
{
    public const short NameMaxLength = 200;
}

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public ICollection<UserTenant> UserTenantList { get; set; } = new List<UserTenant>();
    public ICollection<User> UserList { get; set; } = new List<User>();
}
