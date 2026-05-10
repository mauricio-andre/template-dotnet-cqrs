using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Responses;
using CqrsProject.Core.Identity.UseCases.CreateUser;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class CreateUserTest
{
    [Fact(DisplayName = "Should create a user and publish the creation event")]
    public async Task GivenValidCommand_WhenHandled_ThenPersistUserAndPublishEvent()
    {
        using CoreTestContext context = new CoreTestContext();
        CreateUserHandler handler = new CreateUserHandler(
            new CreateUserValidator(),
            context.UserManager,
            context.Mediator);

        UserResponse response = await handler.Handle(
            new CreateUserCommand("alice", "alice@example.com", "555-0100"),
            CancellationToken.None);

        Assert.Equal("alice", response.UserName);
        Assert.Equal("alice@example.com", response.Email);
        Assert.Equal("555-0100", response.PhoneNumber);
        Assert.True(response.Id != Guid.Empty);

        await context.Mediator.Received(1).Publish(
            Arg.Is<CreateUserEvent>(eventNotification =>
                eventNotification.UserName == "alice"
                && eventNotification.Email == "alice@example.com"),
            Arg.Any<CancellationToken>());
    }
}
