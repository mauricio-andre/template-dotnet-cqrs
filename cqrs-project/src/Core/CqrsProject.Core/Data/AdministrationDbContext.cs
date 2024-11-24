using CqrsProject.Core.Identity;
using CqrsProject.Core.Tenants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Data;

public abstract class AdministrationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    protected AdministrationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserTenant> UserTenants => Set<UserTenant>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
}
