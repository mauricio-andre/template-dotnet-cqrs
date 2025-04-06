namespace CqrsProject.Core.Tenants.Interfaces;

public interface ITenantConnectionProvider
{
    string? GetConnectionStringToCurrentTenant(string connectionName);
    Task LoadAllConnectionStringAsync();
    Task IncludeConnectionStringAsync(Guid tenantId, string connectionName, string keyName);
    void InvalidateConnectionString(Guid tenantId, string connectionName);
}
