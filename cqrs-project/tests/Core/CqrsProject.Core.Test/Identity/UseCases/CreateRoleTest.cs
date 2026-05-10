using CqrsProject.Core.Identity.Responses;
using CqrsProject.Core.Identity.UseCases.CreateRole;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class CreateRoleTest
{
    [Fact(DisplayName = "Should create a role and publish the creation event")]
    public async Task GivenValidCommand_WhenHandled_ThenPersistRoleAndPublishEvent()
    {
        using CoreTestContext context = new CoreTestContext();
        CreateRoleHandler handler = new CreateRoleHandler(
            new CreateRoleValidator(),
            context.RoleManager,
            context.Mediator);

        RoleResponse response = await handler.Handle(new CreateRoleCommand("Admin"), CancellationToken.None);

        Assert.Equal("Admin", response.Name);
        await context.Mediator.Received(1).Publish(
            Arg.Is<CreateRoleEvent>(eventNotification => eventNotification.Name == "Admin"),
            Arg.Any<CancellationToken>());
    }
}
