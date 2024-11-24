using CqrsProject.Core.Data;
using CqrsProject.Core.Identity;
using CqrsProject.Postegre.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Postegre.Data;

public class PostegresAdministrationDbContext : AdministrationDbContext
{
    public PostegresAdministrationDbContext(
        DbContextOptions<AdministrationDbContext> options) : base(options)
    {
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
