using CqrsProject.Core.Identity.Consts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Data;

public static class AdministrationSeedDataConfiguration
{
    public static void Configure(ModelBuilder builder)
    {
        var roleHostAdminId = Guid.Parse("E83BFC7D-61AF-EF11-B120-A830F9D53C51");

        builder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>(IdentityRoleDefaults.HostAdmin)
            {
                Id = roleHostAdminId,
                NormalizedName = IdentityRoleDefaults.HostAdmin.ToUpperInvariant(),
                ConcurrencyStamp = "41f243ef-5fb9-4a2d-ac2d-e0a3c9ed1ed0"
            });

        builder.Entity<IdentityRoleClaim<Guid>>().HasData(
            new IdentityRoleClaim<Guid>()
            {
                Id = -1,
                RoleId = roleHostAdminId,
                ClaimType = IdentityPermissionClaimDefaults.ClaimType,
                ClaimValue = IdentityPermissionClaimDefaults.ManageSelf
            });

        builder.Entity<IdentityRoleClaim<Guid>>().HasData(
            new IdentityRoleClaim<Guid>()
            {
                Id = -2,
                RoleId = roleHostAdminId,
                ClaimType = IdentityPermissionClaimDefaults.ClaimType,
                ClaimValue = IdentityPermissionClaimDefaults.ManageAdministration
            });
    }
}
