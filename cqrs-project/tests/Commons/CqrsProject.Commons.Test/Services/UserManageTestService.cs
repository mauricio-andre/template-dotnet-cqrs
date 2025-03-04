using System.Security.Claims;
using Bogus;
using CqrsProject.Common.Consts;
using CqrsProject.Core.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.Commons.Test.Services;

public class UserManageTestService
{
    private readonly Faker<User> _faker = new Faker<User>()
            .RuleFor(me => me.Email, fake => string.Concat(fake.UniqueIndex, fake.Person.Email))
            .RuleFor(me => me.UserName, fake => string.Concat(fake.UniqueIndex, fake.Person.UserName));

    public async Task<User> CreateMasterAdminUserAsync(IServiceScope scope)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var user = _faker.Generate();

        await userManager.CreateAsync(user);

        await userManager.AddLoginAsync(
            user,
            new UserLoginInfo(
                AuthenticationDefaults.AuthenticationScheme,
                user.Email!,
                AuthenticationDefaults.DisplayName
            ));

        var role = new IdentityRole<Guid>(user.UserName!);

        await roleManager.CreateAsync(role);

        await userManager.AddToRoleAsync(user, role.Name!);

        await roleManager.AddClaimAsync(role, new Claim(
            AuthorizationPermissionClaims.ClaimType,
            AuthorizationPermissionClaims.ManageAdministration));

        await roleManager.AddClaimAsync(role, new Claim(
            AuthorizationPermissionClaims.ClaimType,
            AuthorizationPermissionClaims.ManageExamples));

        await roleManager.AddClaimAsync(role, new Claim(
            AuthorizationPermissionClaims.ClaimType,
            AuthorizationPermissionClaims.ManageSelf));

        await roleManager.AddClaimAsync(role, new Claim(
            AuthorizationPermissionClaims.ClaimType,
            AuthorizationPermissionClaims.ReadExamples));

        return user;
    }

    public async Task<User> CreateUserAsync(IServiceScope scope, IList<string>? permissionClaimList = null)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = _faker.Generate();

        await userManager.CreateAsync(user);

        await userManager.AddLoginAsync(
            user,
            new UserLoginInfo(
                AuthenticationDefaults.AuthenticationScheme,
                user.Email!,
                AuthenticationDefaults.DisplayName
            ));

        if (permissionClaimList?.Count > 0)
            await userManager.AddClaimsAsync(
                user,
                permissionClaimList?.Select(permission => new Claim(
                    AuthorizationPermissionClaims.ClaimType,
                    permission))!);

        return user;
    }
}
