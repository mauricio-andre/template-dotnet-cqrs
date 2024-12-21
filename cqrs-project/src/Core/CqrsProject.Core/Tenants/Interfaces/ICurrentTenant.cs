namespace CqrsProject.Core.Tenants.Interfaces;

public interface ICurrentTenant
{
    void SetCurrentTenantId(Guid? tenantId);
    Guid? GetCurrentTenantId();
    bool IsHost();
}
