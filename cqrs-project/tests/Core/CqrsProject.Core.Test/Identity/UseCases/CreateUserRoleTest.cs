using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.UseCases.CreateUserRole;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class CreateUserRoleTest
{
    [Fact(DisplayName = "Should add a role to a user and publish the creation event")]
    public async Task GivenValidCommand_WhenHandled_ThenAddRoleToUserAndPublishEvent()
    {
        using CoreTestContext context = new CoreTestContext();
        User user = await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");
        IdentityRole<Guid> role = await IdentityTestData.AddRoleAsync(context, "Admin");

        CreateUserRoleHandler handler = new CreateUserRoleHandler(
            new CreateUserRoleValidator(),
            context.Mediator,
            context.Localizer,
            context.UserManager,
            context.RoleManager);

        await handler.Handle(new CreateUserRoleCommand(user.Id, role.Id), CancellationToken.None);

        Assert.Contains("Admin", await context.UserManager.GetRolesAsync(user));
        await context.Mediator.Received(1).Publish(
            Arg.Is<CreateUserRoleEvent>(eventNotification =>
                eventNotification.UserId == user.Id
                && eventNotification.RoleId == role.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should reject missing users when creating a user role")]
    public async Task GivenMissingUser_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        IdentityRole<Guid> role = await IdentityTestData.AddRoleAsync(context, "Admin");

        CreateUserRoleHandler handler = new CreateUserRoleHandler(
            new CreateUserRoleValidator(),
            context.Mediator,
            context.Localizer,
            context.UserManager,
            context.RoleManager);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new CreateUserRoleCommand(Guid.NewGuid(), role.Id), CancellationToken.None));
    }

    [Fact(DisplayName = "Should reject creating a user role when the role is missing")]
    public async Task GivenMissingRole_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        User user = await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");

        CreateUserRoleHandler handler = new CreateUserRoleHandler(
            new CreateUserRoleValidator(),
            context.Mediator,
            context.Localizer,
            context.UserManager,
            context.RoleManager);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new CreateUserRoleCommand(user.Id, Guid.NewGuid()), CancellationToken.None));
    }
}
