using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Interfaces;
using CqrsProject.Postgres.Configurations.Core;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Postgres.Data;

public class PostgresCoreDbContext : CoreDbContext
{
    private readonly ITenantConnectionProvider _tenantConnectionProvider;

    public PostgresCoreDbContext(
        DbContextOptions<CoreDbContext> options,
        ITenantConnectionProvider tenantConnectionProvider) : base(options)
    {
        _tenantConnectionProvider = tenantConnectionProvider;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var connectionString = _tenantConnectionProvider.GetConnectionStringToCurrentTenant("CoreDbContext");
        if (!string.IsNullOrEmpty(connectionString)) optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ExampleEfConfiguration());
    }
}
