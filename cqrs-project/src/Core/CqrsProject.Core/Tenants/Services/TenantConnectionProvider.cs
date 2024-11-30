using CqrsProject.Common.Providers.KeyVaults;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.Core.Tenants;

public class TenantConnectionProvider : ITenantConnectionProvider
{
    private readonly IConfiguration _configuration;
    private readonly ICurrentTenant _currentTenant;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;

    public TenantConnectionProvider(
        IConfiguration configuration,
        ICurrentTenant currentTenant,
        IServiceProvider serviceProvider,
        IDbContextFactory<AdministrationDbContext> dbContextFactory)
    {
        _configuration = configuration;
        _currentTenant = currentTenant;
        _serviceProvider = serviceProvider;
        _dbContextFactory = dbContextFactory;
    }

    public string? GetConnectionStringToCurrentTenant(string connectionName)
    {
        if (_currentTenant.IsHost())
            return _configuration.GetConnectionString(connectionName);

        var connection = ConnectionStringCache.Instance.GetConnectionString(
            _currentTenant.GetCurrentTenantId()!.Value,
            connectionName
        );

        return connection ?? _configuration.GetConnectionString(connectionName);
    }

    public async Task LoadAllConnectionString()
    {
        var keyVaultService = _serviceProvider.GetService<IKeyVaultService>();
        if (keyVaultService == null)
            return;

        var administrationDbContext = await _dbContextFactory.CreateDbContextAsync();
        var tenantConnectionStrings = administrationDbContext.TenantConnectionStrings
            .Where(tenantConnectionString => !tenantConnectionString.Tenant!.IsDeleted)
            .AsAsyncEnumerable();

        await foreach (var tenantConnectionString in tenantConnectionStrings)
        {
            var connectionString = await keyVaultService.GetKeyValueAsync(tenantConnectionString.KeyName);
            ConnectionStringCache.Instance.SetConnectionString(
                tenantConnectionString.TenantId,
                tenantConnectionString.ConnectionName,
                connectionString
            );
        }
    }
}
