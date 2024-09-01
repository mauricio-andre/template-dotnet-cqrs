namespace CqrsProject.Core.Tenants;

public interface ITenantConnectionProvider
{
    string? GetConnectionStringToCurrentTenant();
}
