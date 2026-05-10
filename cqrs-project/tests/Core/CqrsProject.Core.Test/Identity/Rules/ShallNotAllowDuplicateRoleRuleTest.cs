using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Rules;
using CqrsProject.Core.Identity.UseCases.CreateRole;
using CqrsProject.Core.Identity.UseCases.UpdateRole;
using CqrsProject.Core.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Identity.Rules;

public class ShallNotAllowDuplicateRoleRuleTest
{
    [Theory(DisplayName = "Should allow unique role names for every supported event")]
    [MemberData(nameof(GetUniqueRoleCases))]
    public async Task GivenUniqueRole_WhenHandled_ThenCompleteWithoutException(
        Func<CoreTestContext, Task> arrange,
        Func<ShallNotAllowDuplicateRoleRule, Task> act)
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        await arrange(context);
        int initialRoleCount = await dbContext.Roles.CountAsync();
        context.Localizer.Set("message:validation:duplicatedEntity", "Duplicated {0}");
        context.Localizer.Set("message:validation:valueAlreadyUse", "The value {0} is already in use");

        ShallNotAllowDuplicateRoleRule rule = new ShallNotAllowDuplicateRoleRule(
            context.AdministrationDbContextFactory,
            context.Localizer,
            context.RoleManager);

        await act(rule);

        Assert.Equal(initialRoleCount, await dbContext.Roles.CountAsync());
    }

    [Theory(DisplayName = "Should reject duplicated role names for every supported event")]
    [MemberData(nameof(GetDuplicatedRoleCases))]
    public async Task GivenDuplicatedRole_WhenHandled_ThenThrowDuplicatedEntityException(
        Func<CoreTestContext, Task> arrange,
        Func<ShallNotAllowDuplicateRoleRule, Task> act)
    {
        using CoreTestContext context = new CoreTestContext();
        await arrange(context);
        context.Localizer.Set("message:validation:duplicatedEntity", "Duplicated {0}");
        context.Localizer.Set("message:validation:valueAlreadyUse", "The value {0} is already in use");

        ShallNotAllowDuplicateRoleRule rule = new ShallNotAllowDuplicateRoleRule(
            context.AdministrationDbContextFactory,
            context.Localizer,
            context.RoleManager);

        DuplicatedEntityException exception = await Assert.ThrowsAsync<DuplicatedEntityException>(
            () => act(rule));

        Assert.Equal("Duplicated User", exception.Message);
    }

    public static TheoryData<Func<CoreTestContext, Task>, Func<ShallNotAllowDuplicateRoleRule, Task>> GetUniqueRoleCases => new()
    {
        {
            async context => await IdentityTestData.AddRoleAsync(context, "Admin"),
            rule => rule.Handle(new CreateRoleEvent("Viewer"), CancellationToken.None)
        },
        {
            async context => await IdentityTestData.AddRoleAsync(context, "Admin"),
            rule => rule.Handle(new UpdateRoleEvent(Guid.NewGuid(), "Viewer"), CancellationToken.None)
        }
    };

    public static TheoryData<Func<CoreTestContext, Task>, Func<ShallNotAllowDuplicateRoleRule, Task>> GetDuplicatedRoleCases => new()
    {
        {
            async context => await IdentityTestData.AddRoleAsync(context, "Admin"),
            rule => rule.Handle(new CreateRoleEvent("admin"), CancellationToken.None)
        },
        {
            async context =>
            {
                await IdentityTestData.AddRoleAsync(context, "Admin");
                await IdentityTestData.AddRoleAsync(context, "Viewer");
            },
            rule => rule.Handle(new UpdateRoleEvent(Guid.NewGuid(), "viewer"), CancellationToken.None)
        }
    };
}
