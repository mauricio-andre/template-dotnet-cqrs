using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Interfaces;
using CqrsProject.Postgres.Data;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Commons.Test.Database;

public class SqliteCoreDbContext : PostgresCoreDbContext
{
    private readonly SqliteConnectionPull _sqliteConnectionPull;
    public SqliteCoreDbContext(
        DbContextOptions<CoreDbContext> options,
        ITenantConnectionProvider tenantConnectionProvider,
        SqliteConnectionPull sqliteConnectionPull) : base(options, tenantConnectionProvider)
    {
        _sqliteConnectionPull = sqliteConnectionPull;
    }

    protected override void UseTenantConnectionString(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        optionsBuilder.UseSqlite(_sqliteConnectionPull.GetOpenedConnection(connectionString));
    }

}
