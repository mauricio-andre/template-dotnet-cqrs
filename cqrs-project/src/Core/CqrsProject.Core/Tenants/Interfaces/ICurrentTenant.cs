namespace CqrsProject.Core.Tenants.Interfaces;

public interface ICurrentTenant
{
    IDisposable BeginTenantScope(Guid? tenantId);
    Guid? GetCurrentTenantId();
    bool IsHost();
}
