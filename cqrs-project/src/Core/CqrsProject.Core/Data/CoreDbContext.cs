using CqrsProject.Core.Examples.Entities;
using CqrsProject.Core.Tenants.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Data;

public abstract class CoreDbContext : DbContext
{
    private readonly ITenantConnectionProvider _tenantConnectionProvider;

    protected CoreDbContext(
        DbContextOptions options,
        ITenantConnectionProvider tenantConnectionProvider) : base(options)
    {
        _tenantConnectionProvider = tenantConnectionProvider;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var connectionString = _tenantConnectionProvider.GetConnectionStringToCurrentTenant("CoreDbContext");
        if (!string.IsNullOrEmpty(connectionString)) UseTenantConnectionString(optionsBuilder, connectionString);
    }

    /// <summary>
    /// This function should implement the configuration for connecting to the
    /// database according to the driver used by the infrastructure being implemented.
    /// For example, when using PostgreSQL, the expected configuration is
    /// `optionsBuilder.UseNpgsql(connectionString);`
    /// </summary>
    /// <param name="optionsBuilder"></param>
    /// <param name="connectionString"></param>
    protected abstract void UseTenantConnectionString(DbContextOptionsBuilder optionsBuilder, string connectionString);

    public DbSet<Example> Examples => Set<Example>();
}
