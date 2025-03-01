using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Interfaces;
using CqrsProject.Postgres.Data;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Commons.Test.Database;

public class SqliteCoreDbContext : PostgresCoreDbContext
{
    public SqliteCoreDbContext(
        DbContextOptions<CoreDbContext> options,
        ITenantConnectionProvider tenantConnectionProvider) : base(options, tenantConnectionProvider)
    { }

    protected override void UseTenantConnectionString(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        optionsBuilder.UseSqlite(
            SqliteConnectionPull.Instance.GetOpenedConnection(connectionString));
    }

}
