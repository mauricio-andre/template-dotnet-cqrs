using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Postgres.Configurations.Administration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CqrsProject.Postgres.Data;

public class PostgresAdministrationDbContext : AdministrationDbContext
{
    private readonly IConfiguration _configuration;
    public PostgresAdministrationDbContext(
        DbContextOptions<AdministrationDbContext> options,
        IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void UseConnectionString(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("AdministrationDbContext"));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");

        builder.ApplyConfiguration(new TenantEfConfiguration());
        builder.ApplyConfiguration(new UserTenantEfConfiguration());
    }
}
