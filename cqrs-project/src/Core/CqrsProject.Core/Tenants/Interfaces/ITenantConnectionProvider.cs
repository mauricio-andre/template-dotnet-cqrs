namespace CqrsProject.Core.Tenants.Interfaces;

public interface ITenantConnectionProvider
{
    string? GetConnectionStringToCurrentTenant(string connectionName);
    Task LoadAllConnectionString();
}
