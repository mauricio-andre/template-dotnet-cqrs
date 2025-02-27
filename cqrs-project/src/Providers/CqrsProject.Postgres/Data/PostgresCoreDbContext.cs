using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Interfaces;
using CqrsProject.Postgres.Configurations.Core;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Postgres.Data;

public class PostgresCoreDbContext : CoreDbContext
{
    public PostgresCoreDbContext(
        DbContextOptions<CoreDbContext> options,
        ITenantConnectionProvider tenantConnectionProvider) : base(options, tenantConnectionProvider)
    { }

    protected override void UseTenantConnectionString(DbContextOptionsBuilder optionsBuilder, string connectionString)
    {
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ExampleEfConfiguration());
    }
}
