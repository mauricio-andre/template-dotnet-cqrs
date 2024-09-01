
using CqrsProject.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.App.DbMigrator;

public class DbMigratorService : IDbMigratorService
{
    private readonly AdministrationDbContext _administrationDbContext;
    private readonly CoreDbContext _coreDbContext;

    public DbMigratorService(AdministrationDbContext administrationDbContext, CoreDbContext coreDbContext)
    {
        _administrationDbContext = administrationDbContext;
        _coreDbContext = coreDbContext;
    }

    public async Task RunMigrateAsync()
    {
        await _administrationDbContext.Database.MigrateAsync();

        // TODO: implementar loop de execução das migrations para os tenants cadastrados
        await _coreDbContext.Database.MigrateAsync();
    }
}
