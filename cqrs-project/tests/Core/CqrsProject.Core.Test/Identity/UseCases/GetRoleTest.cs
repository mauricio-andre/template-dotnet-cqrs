using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Identity.UseCases.GetRole;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class GetRoleTest
{
    [Fact(DisplayName = "Should return a role when it exists")]
    public async Task GivenExistingRole_WhenHandled_ThenReturnRole()
    {
        using CoreTestContext context = new CoreTestContext();
        var role = await IdentityTestData.AddRoleAsync(context, "Admin");

        GetRoleHandler handler = new GetRoleHandler(
            new GetRoleValidator(),
            context.Localizer,
            context.RoleManager);

        var response = await handler.Handle(new GetRoleQuery(role.Id), CancellationToken.None);

        Assert.Equal("Admin", response.Name);
    }

    [Fact(DisplayName = "Should reject missing roles")]
    public async Task GivenMissingRole_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        GetRoleHandler handler = new GetRoleHandler(
            new GetRoleValidator(),
            context.Localizer,
            context.RoleManager);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new GetRoleQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
