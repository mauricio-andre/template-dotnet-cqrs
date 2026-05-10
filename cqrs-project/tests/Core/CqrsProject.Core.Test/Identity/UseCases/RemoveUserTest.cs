using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.UseCases.RemoveUser;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Common.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class RemoveUserTest
{
    [Fact(DisplayName = "Should mark a user as deleted and remove roles, claims and tenants")]
    public async Task GivenExistingUser_WhenHandled_ThenCleanupAndDeleteUser()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        User user = await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");
        IdentityRole<Guid> role = await IdentityTestData.AddRoleAsync(context, "Admin");
        await IdentityTestData.AddUserToRoleAsync(context, user, role);
        await context.UserManager.AddClaimAsync(user, new System.Security.Claims.Claim("claim", "value"));
        Tenant tenant = await AdministrationTestData.AddTenantAsync(dbContext, name: "Tenant One");
        await AdministrationTestData.AddUserTenantAsync(dbContext, user.Id, tenant.Id);

        RemoveUserHandler handler = new RemoveUserHandler(
            context.AdministrationDbContextFactory,
            new RemoveUserValidator(),
            context.Localizer,
            context.UserManager);

        await handler.Handle(new RemoveUserCommand(user.Id), CancellationToken.None);

        User updated = await context.UserManager.FindByIdAsync(user.Id.ToString()) ?? throw new InvalidOperationException();
        Assert.True(updated.IsDeleted);
        Assert.Empty(await context.UserManager.GetRolesAsync(updated));
        Assert.Empty(await context.UserManager.GetClaimsAsync(updated));
        Assert.Empty(await dbContext.UserTenants.Where(item => item.UserId == user.Id).ToListAsync());
    }

    [Fact(DisplayName = "Should reject removing a missing user")]
    public async Task GivenMissingUser_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        RemoveUserHandler handler = new RemoveUserHandler(
            context.AdministrationDbContextFactory,
            new RemoveUserValidator(),
            context.Localizer,
            context.UserManager);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new RemoveUserCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
