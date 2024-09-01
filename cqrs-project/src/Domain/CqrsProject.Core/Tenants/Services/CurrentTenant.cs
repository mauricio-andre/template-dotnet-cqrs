
namespace CqrsProject.Core.Tenants;

public class CurrentTenant : ICurrentTenant
{
    private Guid? _tenantId;

    public Guid? GetCurrentTenantId() => _tenantId;
    public void SetCurrentTenantId(Guid? tenantId) => _tenantId = tenantId;
}
