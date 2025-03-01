using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.UserTenants.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Data;

public abstract class AdministrationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    protected AdministrationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        UseConnectionString(optionsBuilder);
    }

    /// <summary>
    /// This function should implement the configuration for connecting to the
    /// database according to the driver used by the infrastructure being implemented.
    /// For example, when using PostgreSQL, the expected configuration is
    /// `optionsBuilder.UseNpgsql(connectionString);`
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected abstract void UseConnectionString(DbContextOptionsBuilder optionsBuilder);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        AdministrationSeedDataConfiguration.Configure(builder);
    }

    public DbSet<UserTenant> UserTenants => Set<UserTenant>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantConnectionString> TenantConnectionStrings => Set<TenantConnectionString>();
}
