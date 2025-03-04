using System.Diagnostics;
using CqrsProject.App.DbMigrator.Interfaces;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CqrsProject.App.DbMigrator.Services;

public class DbMigratorService : IDbMigratorService
{
    private readonly IDbContextFactory<AdministrationDbContext> _administrationDbContextFactory;
    private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;
    private readonly ICurrentTenant _currentTenant;
    private readonly IConfiguration _configuration;

    public DbMigratorService(
        IDbContextFactory<AdministrationDbContext> administrationDbContextFactory,
        IDbContextFactory<CoreDbContext> coreDbContextFactory,
        ICurrentTenant currentTenant,
        IConfiguration configuration)
    {
        _administrationDbContextFactory = administrationDbContextFactory;
        _coreDbContextFactory = coreDbContextFactory;
        _currentTenant = currentTenant;
        _configuration = configuration;
    }

    public async Task RunMigrateAsync(CancellationToken cancellationToken = default)
    {
        await RunMigrateAdministration(cancellationToken);

        await RunMigrateCore(cancellationToken);
    }

    private async Task RunMigrateAdministration(CancellationToken cancellationToken)
    {
        Activity.Current?.AddEvent(new ActivityEvent("Start migration Administration"));
        var dbContext = await _administrationDbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    private async Task RunMigrateCore(CancellationToken cancellationToken)
    {
        await RunMigrateCoreHost(cancellationToken);
        await RunMigrateCoreTenants(cancellationToken);
    }

    private async Task RunMigrateCoreHost(CancellationToken cancellationToken)
    {
        if (IsHostDatabaseConfigured())
        {
            var dbContext = await _coreDbContextFactory.CreateDbContextAsync();
            Activity.Current?.AddEvent(new ActivityEvent("Start migration Core Host"));
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
    }

    private bool IsHostDatabaseConfigured()
    {
        return !string.IsNullOrEmpty(_configuration.GetConnectionString("CoreDbContext"));
    }

    private async Task RunMigrateCoreTenants(CancellationToken cancellationToken)
    {
        Activity.Current?.AddEvent(new ActivityEvent("Searching Tenants to run Migration"));
        var administrationDbContext = await _administrationDbContextFactory.CreateDbContextAsync();
        var tenants = administrationDbContext.Tenants
            .Where(tenant => !tenant.IsDeleted)
            .Select(tenant => tenant.Id)
            .AsAsyncEnumerable();

        await foreach (var tenantId in tenants)
        {
            using (_currentTenant.BeginTenantScope(tenantId))
            {
                Activity.Current?.AddEvent(new ActivityEvent("Start migration Core Tenant"));
                var coreDbContext = await _coreDbContextFactory.CreateDbContextAsync();
                await coreDbContext.Database.MigrateAsync(cancellationToken);
            }
        }
    }
}
