using CqrsProject.App.DbMigrator.Interfaces;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Extensions;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CqrsProject.Core.Examples.Entities;

namespace CqrsProject.App.DbMigratorTest;

public class DbMigratorTest
{
    [Fact(DisplayName = "Should shut down application after migration runner completes")]
    public async Task GivenMigrationRunner_WhenItCompletes_ThenApplicationShutsDown()
    {
        var host = DbMigratorHostFactory.DbMigratorHost();
        var worker = host.Services.GetRequiredService<IHostedService>();
        var applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

        await worker.StartAsync(default);

        var count = 0;
        var max = 10;
        do
        {
            count++;
            await Task.Delay(TimeSpan.FromSeconds(3));
        } while (!applicationLifetime.ApplicationStopping.IsCancellationRequested && count <= max);

        Assert.True(applicationLifetime.ApplicationStopping.IsCancellationRequested);
    }

    [Fact(DisplayName = "Should create database and apply migrations correctly for tenants")]
    public async Task GivenTenantConnections_WhenMigratorRuns_ThenCreateDatabase()
    {
        var host = DbMigratorHostFactory.DbMigratorHost();
        var currentTenant = host.Services.GetRequiredService<ICurrentTenant>();
        var dbMigratorService = host.Services.GetRequiredService<IDbMigratorService>();

        await dbMigratorService.RunMigrateAsync();

        var dbContext = host.Services.GetRequiredService<AdministrationDbContext>();
        var tenant = new Tenant()
        {
            Name = "MyTeste",
            TenantConnectionStringList = new List<TenantConnectionString>()
            {
                new TenantConnectionString()
                {
                    ConnectionName = "CoreDbContext",
                    KeyName = "MyTesteDb"
                }
            }
        };

        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.SaveChangesAsync();

        host.LoadMultiTenantConnections();
        await dbMigratorService.RunMigrateAsync();

        var dbContextHostFactory = host.Services.GetRequiredService<IDbContextFactory<CoreDbContext>>();
        var dbContextHost = await dbContextHostFactory.CreateDbContextAsync();

        await dbContextHost.Examples.AddAsync(new Example() { Name = "inHost" });
        await dbContextHost.SaveChangesAsync();

        var countHost = await dbContextHost.Examples.CountAsync();
        Assert.Equal(1, countHost);

        using (currentTenant.BeginTenantScope(tenant.Id))
        {
            var dbContextTenantFactory = host.Services.GetRequiredService<IDbContextFactory<CoreDbContext>>();
            var dbContextTenant = await dbContextTenantFactory.CreateDbContextAsync();

            var countBefore = await dbContextTenant.Examples.CountAsync();
            Assert.Equal(0, countBefore);

            await dbContextTenant.Examples.AddAsync(new Example() { Name = "inTenant" });
            await dbContextTenant.SaveChangesAsync();

            var countAfter = await dbContextTenant.Examples.CountAsync();
            Assert.Equal(1, countAfter);
        }
    }
}
