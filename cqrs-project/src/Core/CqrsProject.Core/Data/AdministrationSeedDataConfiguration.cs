using CqrsProject.Core.Identity.Consts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Data;

public static class AdministrationSeedDataConfiguration
{
    public static void Configure(ModelBuilder builder)
    {
        builder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>(IdentityRoleDefaults.HostAdmin)
            {
                Id = Guid.NewGuid(),
                NormalizedName = IdentityRoleDefaults.HostAdmin.ToUpperInvariant()
            });

        builder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>(IdentityRoleDefaults.TenantAdmin)
            {
                Id = Guid.NewGuid(),
                NormalizedName = IdentityRoleDefaults.TenantAdmin.ToUpperInvariant()
            });

        builder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>(IdentityRoleDefaults.Client)
            {
                Id = Guid.NewGuid(),
                NormalizedName = IdentityRoleDefaults.Client.ToUpperInvariant()
            });

    }
}
