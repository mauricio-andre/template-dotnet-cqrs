using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants;
using CqrsProject.Postegre.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Postegre.Data;

public class PostegresCoreDbContext : CoreDbContext
{
    private readonly ITenantConnectionProvider _tenantConnectionProvider;

    public PostegresCoreDbContext(
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
