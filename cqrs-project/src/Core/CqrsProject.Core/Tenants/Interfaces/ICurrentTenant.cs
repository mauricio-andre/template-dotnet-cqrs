namespace CqrsProject.Core.Tenants;

public interface ICurrentTenant
{
    void SetCurrentTenantId(Guid? tenantId);
    Guid? GetCurrentTenantId();
    bool IsHost();
    ValueTask<string?> GetConnectionKeyNameAsync(string connectionName);
}
