using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Identity.Responses;
using CqrsProject.Core.Identity.UseCases.UpdateRole;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using NSubstitute;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class UpdateRoleTest
{
    [Fact(DisplayName = "Should update a role and publish the update event")]
    public async Task GivenExistingRole_WhenHandled_ThenUpdateRoleAndPublishEvent()
    {
        using CoreTestContext context = new CoreTestContext();
        var role = await IdentityTestData.AddRoleAsync(context, "Admin");

        UpdateRoleHandler handler = new UpdateRoleHandler(
            new UpdateRoleValidator(),
            context.Localizer,
            context.RoleManager,
            context.Mediator);

        RoleResponse response = await handler.Handle(new UpdateRoleCommand(role.Id, "AdminTwo"), CancellationToken.None);

        Assert.Equal("AdminTwo", response.Name);
        await context.Mediator.Received(1).Publish(
            Arg.Is<UpdateRoleEvent>(eventNotification => eventNotification.Id == role.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should reject updating missing roles")]
    public async Task GivenMissingRole_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        UpdateRoleHandler handler = new UpdateRoleHandler(
            new UpdateRoleValidator(),
            context.Localizer,
            context.RoleManager,
            context.Mediator);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new UpdateRoleCommand(Guid.NewGuid(), "AdminTwo"), CancellationToken.None));
    }
}
