using CqrsProject.Core.Identity.UseCases.RemoveRoleClaim;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using CqrsProject.Common.Exceptions;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class RemoveRoleClaimTest
{
    [Fact(DisplayName = "Should remove a claim from a role")]
    public async Task GivenExistingRoleClaim_WhenHandled_ThenRemoveIt()
    {
        using CoreTestContext context = new CoreTestContext();
        var role = await IdentityTestData.AddRoleAsync(context, "Admin");
        await IdentityTestData.AddRoleClaimAsync(context, role, "permission", "read");

        RemoveRoleClaimHandler handler = new RemoveRoleClaimHandler(
            new RemoveRoleClaimValidator(),
            context.Localizer,
            context.RoleManager);

        await handler.Handle(new RemoveRoleClaimCommand(role.Id, "permission", "read"), CancellationToken.None);

        Assert.Empty(await context.RoleManager.GetClaimsAsync(role));
    }

    [Fact(DisplayName = "Should reject removing a role claim when the role is missing")]
    public async Task GivenMissingRole_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();

        RemoveRoleClaimHandler handler = new RemoveRoleClaimHandler(
            new RemoveRoleClaimValidator(),
            context.Localizer,
            context.RoleManager);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new RemoveRoleClaimCommand(Guid.NewGuid(), "permission", "read"), CancellationToken.None));
    }
}
