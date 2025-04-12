using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Common.Providers.KeyVaults.Interfaces;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Caches;
using CqrsProject.Core.Tenants.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace CqrsProject.Core.Tenants.Services;

public class TenantConnectionProvider : ITenantConnectionProvider
{
    private readonly IConfiguration _configuration;
    private readonly ICurrentTenant _currentTenant;
    private readonly IDbContextFactory<AdministrationDbContext> _dbContextFactory;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;
    private readonly IKeyVaultService? _keyVaultService;

    public TenantConnectionProvider(
        IConfiguration configuration,
        ICurrentTenant currentTenant,
        IServiceProvider serviceProvider,
        IDbContextFactory<AdministrationDbContext> dbContextFactory,
        IStringLocalizer<CqrsProjectResource> stringLocalizer,
        IKeyVaultService? keyVaultService = null)
    {
        _configuration = configuration;
        _currentTenant = currentTenant;
        _dbContextFactory = dbContextFactory;
        _stringLocalizer = stringLocalizer;
        _keyVaultService = keyVaultService;
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

    public async Task LoadAllConnectionStringAsync()
    {
        if (_keyVaultService == null)
            return;

        var administrationDbContext = await _dbContextFactory.CreateDbContextAsync();
        var tenantConnectionStrings = administrationDbContext.TenantConnectionStrings
            .Where(tenantConnectionString => !tenantConnectionString.Tenant!.IsDeleted)
            .AsAsyncEnumerable();

        await foreach (var tenantConnectionString in tenantConnectionStrings)
        {
            await IncludeConnectionStringAsync(
                tenantConnectionString.TenantId,
                tenantConnectionString.ConnectionName,
                tenantConnectionString.KeyName);
        }
    }

    public async Task IncludeConnectionStringAsync(Guid tenantId, string connectionName, string keyName)
    {
        if (_keyVaultService == null)
            return;

        var connectionString = await _keyVaultService.GetKeyValueAsync(keyName);

        if (string.IsNullOrEmpty(connectionString))
            throw new ConnectionStringKeyNotFoundException(_stringLocalizer, tenantId.ToString(), keyName);

        ConnectionStringCache.Instance.SetConnectionString(
            tenantId,
            connectionName,
            connectionString
        );
    }

    public void InvalidateConnectionString(Guid tenantId, string connectionName)
    {
        ConnectionStringCache.Instance.RemoveConnectionString(tenantId, connectionName);
    }
}
