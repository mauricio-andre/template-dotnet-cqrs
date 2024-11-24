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

        // TODO: gera thread lock, pensar em alternativas
        /*
            Uma opção é criar um singleton com a lista de conexões disponíveis
            que é carregado no start da aplicação, e que tem disparo de reload
            sob demanda baseado no tempo da conexão armazenada, algo parecido
            com que o nextjs faz para páginas pré renderizadas, dessa foma
            aqui seria usado sempre a função sincrona que retorna dados já
            consultados em outro processo
        */
        return GetConnectionStringFromVault().GetAwaiter().GetResult();
    }

    private async ValueTask<string?> GetConnectionStringFromVault()
    {
        var keyVaultService = _serviceProvider.GetService<IKeyVaultService>();

        if (keyVaultService == null)
            return _configuration.GetConnectionString(ConnectionName);

        var keyName = await _currentTenant.GetConnectionKeyNameAsync(ConnectionName);
        var connectionString = !string.IsNullOrEmpty(keyName)
            ? await keyVaultService.GetKeyValueAsync<string?>(keyName)
            : null;

        return connectionString ?? _configuration.GetConnectionString(ConnectionName);
    }
}
