using CqrsProject.Common.Providers.KeyVaults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.Core.Tenants;

public class TenantConnectionProvider : ITenantConnectionProvider
{
    private readonly IConfiguration _configuration;
    private readonly ICurrentTenant _currentTenant;
    private readonly IServiceProvider _serviceProvider;
    private const string ConnectionName = "CoreDbContext";

    public TenantConnectionProvider(
        IConfiguration configuration,
        ICurrentTenant currentTenant,
        IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _currentTenant = currentTenant;
        _serviceProvider = serviceProvider;
    }

    public string? GetConnectionStringToCurrentTenant()
    {
        if (_currentTenant.IsHost())
            return _configuration.GetConnectionString(ConnectionName);

        var keyName = _currentTenant.GetConnectionKeyNameAsync(ConnectionName).GetAwaiter().GetResult();
        var connectionString = !string.IsNullOrEmpty(keyName)
            ? _serviceProvider
                .GetService<IKeyVaultService>()
                ?.GetKeyValueAsync<string?>(keyName)
                .GetAwaiter()
                .GetResult()
            : null;

        return connectionString ?? _configuration.GetConnectionString(ConnectionName);
    }
}
