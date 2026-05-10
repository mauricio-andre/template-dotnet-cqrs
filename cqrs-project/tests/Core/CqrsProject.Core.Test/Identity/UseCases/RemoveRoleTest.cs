using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Identity.UseCases.RemoveRole;
using CqrsProject.Core.Test.Identity;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class RemoveRoleTest
{
    [Fact(DisplayName = "Should delete a role")]
    public async Task GivenExistingRole_WhenHandled_ThenDeleteIt()
    {
        using CoreTestContext context = new CoreTestContext();
        var role = await IdentityTestData.AddRoleAsync(context, "Admin");

        RemoveRoleHandler handler = new RemoveRoleHandler(
            new RemoveRoleValidator(),
            context.Localizer,
            context.RoleManager);

        await handler.Handle(new RemoveRoleCommand(role.Id), CancellationToken.None);

        Assert.Null(await context.RoleManager.FindByIdAsync(role.Id.ToString()));
    }

    [Fact(DisplayName = "Should reject missing roles when removing")]
    public async Task GivenMissingRole_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        RemoveRoleHandler handler = new RemoveRoleHandler(
            new RemoveRoleValidator(),
            context.Localizer,
            context.RoleManager);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new RemoveRoleCommand(Guid.NewGuid()), CancellationToken.None));
    }
}

