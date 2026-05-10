using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.UseCases.RemoveUserRole;
using CqrsProject.Core.Test.Identity;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class RemoveUserRoleTest
{
    [Fact(DisplayName = "Should remove a role from a user")]
    public async Task GivenExistingUserRole_WhenHandled_ThenRemoveRoleFromUser()
    {
        using CoreTestContext context = new CoreTestContext();
        User user = await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");
        IdentityRole<Guid> role = await IdentityTestData.AddRoleAsync(context, "Admin");
        await IdentityTestData.AddUserToRoleAsync(context, user, role);

        RemoveUserRoleHandler handler = new RemoveUserRoleHandler(
            new RemoveUserRoleValidator(),
            context.Localizer,
            context.UserManager,
            context.RoleManager);

        await handler.Handle(new RemoveUserRoleCommand(user.Id, role.Id), CancellationToken.None);

        Assert.Empty(await context.UserManager.GetRolesAsync(user));
    }

    [Fact(DisplayName = "Should reject removing a user role when the user is missing")]
    public async Task GivenMissingUser_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        IdentityRole<Guid> role = await IdentityTestData.AddRoleAsync(context, "Admin");

        RemoveUserRoleHandler handler = new RemoveUserRoleHandler(
            new RemoveUserRoleValidator(),
            context.Localizer,
            context.UserManager,
            context.RoleManager);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new RemoveUserRoleCommand(Guid.NewGuid(), role.Id), CancellationToken.None));
    }

    [Fact(DisplayName = "Should reject removing a user role when the role is missing")]
    public async Task GivenMissingRole_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        User user = await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");

        RemoveUserRoleHandler handler = new RemoveUserRoleHandler(
            new RemoveUserRoleValidator(),
            context.Localizer,
            context.UserManager,
            context.RoleManager);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new RemoveUserRoleCommand(user.Id, Guid.NewGuid()), CancellationToken.None));
    }

    [Fact(DisplayName = "Should reject removing a user role when the user is deleted")]
    public async Task GivenDeletedUser_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        User user = await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com", isDeleted: true);
        IdentityRole<Guid> role = await IdentityTestData.AddRoleAsync(context, "Admin");

        RemoveUserRoleHandler handler = new RemoveUserRoleHandler(
            new RemoveUserRoleValidator(),
            context.Localizer,
            context.UserManager,
            context.RoleManager);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new RemoveUserRoleCommand(user.Id, role.Id), CancellationToken.None));
    }
}

