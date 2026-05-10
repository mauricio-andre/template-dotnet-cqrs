using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.UseCases.CreateRoleClaim;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class CreateRoleClaimTest
{
    [Fact(DisplayName = "Should add a claim to a role and publish the claim event")]
    public async Task GivenValidCommand_WhenHandled_ThenAddClaimAndPublishEvent()
    {
        using CoreTestContext context = new CoreTestContext();
        var role = await IdentityTestData.AddRoleAsync(context, "Admin");
        context.CurrentIdentity.GetRoles().Returns(["Admin"]);
        context.CurrentIdentity.GetLocalIdentityId().Returns(Guid.NewGuid());

        CreateRoleClaimHandler handler = new CreateRoleClaimHandler(
            new CreateRoleClaimValidator(),
            context.RoleManager,
            context.Localizer,
            context.Mediator,
            context.CurrentIdentity);

        await handler.Handle(new CreateRoleClaimCommand(role.Id, "permission", "read"), CancellationToken.None);

        Assert.Contains(await context.RoleManager.GetClaimsAsync(role), claim => claim.Type == "permission" && claim.Value == "read");
        await context.Mediator.Received(1).Publish(
            Arg.Is<CreateRoleClaimEvent>(eventNotification =>
                eventNotification.RoleId == role.Id
                && eventNotification.ClaimType == "permission"
                && eventNotification.ClaimValue == "read"),
            Arg.Any<CancellationToken>());
        await context.Mediator.Received(1).Publish(
            Arg.Is<CreateRoleClaimForYourselfEvent>(eventNotification => eventNotification.UserId == context.CurrentIdentity.GetLocalIdentityId()),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should reject creating a role claim when the role is missing")]
    public async Task GivenMissingRole_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        context.CurrentIdentity.GetRoles().Returns(Array.Empty<string>());

        CreateRoleClaimHandler handler = new CreateRoleClaimHandler(
            new CreateRoleClaimValidator(),
            context.RoleManager,
            context.Localizer,
            context.Mediator,
            context.CurrentIdentity);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new CreateRoleClaimCommand(Guid.NewGuid(), "permission", "read"), CancellationToken.None));
    }

    [Fact(DisplayName = "Should create a role claim without publishing the self-claim event when the user is not in the role")]
    public async Task GivenUserNotInRole_WhenHandled_ThenAddClaimWithoutPublishingSelfEvent()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        IdentityRole<Guid> role = await IdentityTestData.AddRoleAsync(context, "Admin");
        int initialRoleClaimCount = await dbContext.RoleClaims.CountAsync();
        context.CurrentIdentity.GetRoles().Returns(Array.Empty<string>());
        context.CurrentIdentity.GetLocalIdentityId().Returns(Guid.NewGuid());

        CreateRoleClaimHandler handler = new CreateRoleClaimHandler(
            new CreateRoleClaimValidator(),
            context.RoleManager,
            context.Localizer,
            context.Mediator,
            context.CurrentIdentity);

        await handler.Handle(new CreateRoleClaimCommand(role.Id, "permission", "read"), CancellationToken.None);

        Assert.Equal(initialRoleClaimCount + 1, await dbContext.RoleClaims.CountAsync());
        await context.Mediator.Received(1).Publish(
            Arg.Is<CreateRoleClaimEvent>(eventNotification =>
                eventNotification.RoleId == role.Id
                && eventNotification.ClaimType == "permission"
                && eventNotification.ClaimValue == "read"),
            Arg.Any<CancellationToken>());
        await context.Mediator.DidNotReceive().Publish(
            Arg.Is<CreateRoleClaimForYourselfEvent>(eventNotification => eventNotification.UserId == context.CurrentIdentity.GetLocalIdentityId()),
            Arg.Any<CancellationToken>());
    }

}
