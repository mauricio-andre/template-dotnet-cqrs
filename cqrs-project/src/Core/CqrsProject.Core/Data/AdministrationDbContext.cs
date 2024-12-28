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

    public DbSet<UserTenant> UserTenants => Set<UserTenant>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantConnectionString> TenantConnectionStrings => Set<TenantConnectionString>();
}
