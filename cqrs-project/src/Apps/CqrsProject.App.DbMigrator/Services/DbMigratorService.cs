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

    public DbMigratorService(
        IDbContextFactory<AdministrationDbContext> administrationDbContextFactory,
        IDbContextFactory<CoreDbContext> coreDbContextFactory,
        ICurrentTenant currentTenant)
    {
        _administrationDbContextFactory = administrationDbContextFactory;
        _coreDbContextFactory = coreDbContextFactory;
        _currentTenant = currentTenant;
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
        var dbContext = await _coreDbContextFactory.CreateDbContextAsync();

        if (IsHostDatabaseConfigured(dbContext.Database))
        {
            Activity.Current?.AddEvent(new ActivityEvent("Start migration Core Host"));
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
    }

    private static bool IsHostDatabaseConfigured(DatabaseFacade database)
    {
        try
        {
            return !string.IsNullOrEmpty(database.GetConnectionString());
        }
        catch (Exception)
        {
            return false;
        }
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
