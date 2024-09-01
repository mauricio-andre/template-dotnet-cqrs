using Microsoft.Extensions.Configuration;

namespace CqrsProject.Core.Tenants;

public class TenantConnectionProvider : ITenantConnectionProvider
{
    private readonly IConfiguration _configuration;
    private readonly ICurrentTenant _currentTenant;

    public TenantConnectionProvider(IConfiguration configuration, ICurrentTenant currentTenant)
    {
        _configuration = configuration;
        _currentTenant = currentTenant;
    }

    public string? GetConnectionStringToCurrentTenant()
    {
        if (_currentTenant.GetCurrentTenantId() == null)
            return _configuration.GetConnectionString("CoreDbContext");

        // TODO: Resgatar a connection string do tenant selecionado de preferencia de algum serviço de vault
        return _configuration.GetConnectionString("CoreDbContext");
    }
}
