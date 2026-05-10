using System.Security.Claims;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.UserTenants.Entities;
using Microsoft.AspNetCore.Identity;

namespace CqrsProject.Core.Test.Identity;

internal static class IdentityTestData
{
    public static async Task<User> AddUserAsync(
        CoreTestContext context,
        string? userName = null,
        string? email = null,
        string? phoneNumber = null,
        bool isDeleted = false)
    {
        User user = new User
        {
            UserName = userName ?? $"user-{Guid.NewGuid():N}",
            Email = email ?? $"{Guid.NewGuid():N}@example.com",
            PhoneNumber = phoneNumber,
            CreationTime = DateTimeOffset.UtcNow,
            IsDeleted = isDeleted
        };

        IdentityResult result = await context.UserManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));

        return user;
    }

    public static async Task<IdentityRole<Guid>> AddRoleAsync(
        CoreTestContext context,
        string? name = null)
    {
        IdentityRole<Guid> role = new IdentityRole<Guid>
        {
            Name = name ?? $"role-{Guid.NewGuid():N}"
        };

        IdentityResult result = await context.RoleManager.CreateAsync(role);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));

        return role;
    }

    public static async Task AddUserToRoleAsync(CoreTestContext context, User user, IdentityRole<Guid> role)
    {
        IdentityResult result = await context.UserManager.AddToRoleAsync(user, role.Name!);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
    }

    public static async Task AddRoleClaimAsync(
        CoreTestContext context,
        IdentityRole<Guid> role,
        string claimType,
        string claimValue)
    {
        IdentityResult result = await context.RoleManager.AddClaimAsync(role, new Claim(claimType, claimValue));
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
    }

    public static async Task AddLoginAsync(CoreTestContext context, User user, string providerKey)
    {
        IdentityResult result = await context.UserManager.AddLoginAsync(
            user,
            new UserLoginInfo(
                AuthenticationDefaults.AuthenticationScheme,
                providerKey,
                AuthenticationDefaults.DisplayName));

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
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
