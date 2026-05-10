using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Responses;
using CqrsProject.Core.Identity.UseCases.UpdateUser;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class UpdateUserTest
{
    [Fact(DisplayName = "Should update a user and publish the update event")]
    public async Task GivenExistingUser_WhenHandled_ThenUpdateUserAndPublishEvent()
    {
        using CoreTestContext context = new CoreTestContext();
        User user = await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com", phoneNumber: "111");

        UpdateUserHandler handler = new UpdateUserHandler(
            new UpdateUserValidator(),
            context.Localizer,
            context.UserManager,
            context.Mediator);

        UserResponse response = await handler.Handle(
            new UpdateUserCommand(user.Id, "alice-two", "alice-two@example.com", "222"),
            CancellationToken.None);

        Assert.Equal("alice-two", response.UserName);
        Assert.Equal("alice-two@example.com", response.Email);
        Assert.Equal("222", response.PhoneNumber);
        Assert.NotNull(response.LastModificationTime);

        await context.Mediator.Received(1).Publish(
            Arg.Is<UpdateUserEvent>(eventNotification => eventNotification.Id == user.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should reject updating a missing user")]
    public async Task GivenMissingUser_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        UpdateUserHandler handler = new UpdateUserHandler(
            new UpdateUserValidator(),
            context.Localizer,
            context.UserManager,
            context.Mediator);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new UpdateUserCommand(Guid.NewGuid(), "alice", "alice@example.com", null), CancellationToken.None));
    }
}
