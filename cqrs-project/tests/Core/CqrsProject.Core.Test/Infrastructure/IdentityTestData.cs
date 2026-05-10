using System.Security.Claims;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Infrastructure;

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
}
