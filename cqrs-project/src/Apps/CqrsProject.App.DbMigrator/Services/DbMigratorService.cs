
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.App.DbMigrator;

public class DbMigratorService : IDbMigratorService
{
    private readonly IDbContextFactory<AdministrationDbContext> _administrationDbContextFactory;
    private readonly IDbContextFactory<CoreDbContext> _coreDbContextFactory;
    private readonly ICurrentTenant _currentTenant;
    private readonly ITenantConnectionProvider _tenantConnectionProvider;

    public DbMigratorService(
        IDbContextFactory<AdministrationDbContext> administrationDbContextFactory,
        IDbContextFactory<CoreDbContext> coreDbContextFactory,
        ICurrentTenant currentTenant,
        ITenantConnectionProvider tenantConnectionProvider)
    {
        _administrationDbContextFactory = administrationDbContextFactory;
        _coreDbContextFactory = coreDbContextFactory;
        _currentTenant = currentTenant;
        _tenantConnectionProvider = tenantConnectionProvider;
    }

    public async Task RunMigrateAsync(CancellationToken cancellationToken = default)
    {
        await RunMigrateAdministration(cancellationToken);

        await _tenantConnectionProvider.LoadAllConnectionString();

        await RunMigrateCore(cancellationToken);
    }

    private async Task RunMigrateAdministration(CancellationToken cancellationToken)
    {
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
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    private async Task RunMigrateCoreTenants(CancellationToken cancellationToken)
    {
        var administrationDbContext = await _administrationDbContextFactory.CreateDbContextAsync();
        var tenants = administrationDbContext.Tenants
            .Where(tenant => !tenant.IsDeleted)
            .Select(tenant => tenant.Id)
            .AsAsyncEnumerable();

        await foreach (var tenantId in tenants)
        {
            _currentTenant.SetCurrentTenantId(tenantId);
            var coreDbContext = await _coreDbContextFactory.CreateDbContextAsync();
            await coreDbContext.Database.MigrateAsync(cancellationToken);
        }
    }
}
