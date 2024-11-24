namespace CqrsProject.Core.Tenants;

public static class TenantConnectionStringConstrains
{
    public const short ConnectionNameMaxLength = 50;
    public const short KeyNameMaxLength = 200;
}

public class TenantConnectionString
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ConnectionName { get; set; } = string.Empty;
    public string KeyName { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
}
