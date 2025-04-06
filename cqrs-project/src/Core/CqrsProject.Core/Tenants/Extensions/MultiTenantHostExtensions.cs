using CqrsProject.Core.Tenants.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CqrsProject.Core.Tenants.Extensions;

public static class MultiTenantHostExtensions
{
    public static void LoadMultiTenantConnections(this IHost app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<ITenantConnectionProvider>();
            service.LoadAllConnectionStringAsync();
        }
    }
}
