namespace CqrsProject.Core.Tenants;

public class CurrentTenant : ICurrentTenant
{
    private Guid? _tenantId;

    public CurrentTenant()
    {
    }

    public Guid? GetCurrentTenantId() => _tenantId;
    public void SetCurrentTenantId(Guid? tenantId) => _tenantId = tenantId;
    public bool IsHost() => _tenantId == null;
}
