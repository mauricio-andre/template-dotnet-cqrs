using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.UserTenants.Entities;

namespace CqrsProject.Core.Test.UserTenants;

internal static class UserTenantsTestData
{
    public static async Task<User> AddUserAsync(
        AdministrationDbContext dbContext,
        Guid? id = null,
        string? userName = null,
        bool isDeleted = false)
    {
        string currentUserName = userName ?? $"user-{Guid.NewGuid():N}";
        User user = new User
        {
            Id = id ?? Guid.NewGuid(),
            UserName = currentUserName,
            NormalizedUserName = currentUserName.ToUpperInvariant(),
            Email = $"{Guid.NewGuid():N}@example.com",
            NormalizedEmail = $"{Guid.NewGuid():N}@EXAMPLE.COM",
            SecurityStamp = Guid.NewGuid().ToString("N"),
            ConcurrencyStamp = Guid.NewGuid().ToString("N"),
            IsDeleted = isDeleted
        };

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    public static async Task<Tenant> AddTenantAsync(
        AdministrationDbContext dbContext,
        Guid? id = null,
        string? name = null,
        bool isDeleted = false)
    {
        Tenant tenant = new Tenant
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? $"Tenant-{Guid.NewGuid():N}",
            IsDeleted = isDeleted
        };

        await dbContext.Tenants.AddAsync(tenant);
        await dbContext.SaveChangesAsync();

        return tenant;
    }

    public static async Task<UserTenant> AddUserTenantAsync(
        AdministrationDbContext dbContext,
        Guid userId,
        Guid tenantId,
        Guid? creatorId = null)
    {
        UserTenant userTenant = new UserTenant
        {
            UserId = userId,
            TenantId = tenantId,
            CreationTime = DateTimeOffset.UtcNow,
            CreatorId = creatorId
        };

        await dbContext.UserTenants.AddAsync(userTenant);
        await dbContext.SaveChangesAsync();

        return userTenant;
    }
}
