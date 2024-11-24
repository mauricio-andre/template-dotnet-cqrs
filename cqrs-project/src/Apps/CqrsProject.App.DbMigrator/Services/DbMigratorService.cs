
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.App.DbMigrator;

public class DbMigratorService : IDbMigratorService
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly CoreDbContext _coreDbContext;
    private readonly ICurrentTenant _currentTenant;

    public DbMigratorService(
        AdministrationDbContext administrationDbContext,
        CoreDbContext coreDbContext,
        ICurrentTenant currentTenant)
    {
        _administrationDbContext = administrationDbContext;
        _coreDbContext = coreDbContext;
        _currentTenant = currentTenant;
    }

    public async Task RunMigrateAsync(CancellationToken cancellationToken = default)
    {
        await RunMigrateAdministration(cancellationToken);

        await RunMigrateCore(cancellationToken);
    }

    private Task RunMigrateAdministration(CancellationToken cancellationToken)
    {
        return _administrationDbContext.Database.MigrateAsync(cancellationToken);
    }

    private async Task RunMigrateCore(CancellationToken cancellationToken)
    {
        await RunMigrateCoreHost(cancellationToken);
        await RunMigrateCoreTenants(cancellationToken);
    }

    private Task RunMigrateCoreHost(CancellationToken cancellationToken)
    {
        return _coreDbContext.Database.MigrateAsync(cancellationToken);
    }

    private async Task RunMigrateCoreTenants(CancellationToken cancellationToken)
    {
        var tenantList = await _administrationDbContext.Tenants.ToListAsync(cancellationToken);

        foreach (var tenant in tenantList)
        {
            _currentTenant.SetCurrentTenantId(tenant.Id);
            await _coreDbContext.Database.MigrateAsync(cancellationToken);
        }
    }
}
