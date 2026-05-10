using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Rules;
using CqrsProject.Core.Identity.UseCases.CreateUserRole;
using CqrsProject.Core.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Identity.Rules;

public class ShallNotAllowDuplicateUserRoleRuleTest
{
    [Fact(DisplayName = "Should allow a unique user role pair")]
    public async Task GivenUniqueUserRole_WhenHandled_ThenCompleteWithoutException()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        var user = await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");
        var role = await IdentityTestData.AddRoleAsync(context, "Admin");

        ShallNotAllowDuplicateUserRoleRule rule = new ShallNotAllowDuplicateUserRoleRule(
            context.AdministrationDbContextFactory,
            context.Localizer);

        await rule.Handle(new CreateUserRoleEvent(user.Id, role.Id), CancellationToken.None);

        Assert.Empty(await dbContext.UserRoles.ToListAsync());
    }

    [Fact(DisplayName = "Should reject duplicate user role pairs")]
    public async Task GivenDuplicatedUserRole_WhenHandled_ThenThrowDuplicatedEntityException()
    {
        using CoreTestContext context = new CoreTestContext();
        var user = await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");
        var role = await IdentityTestData.AddRoleAsync(context, "Admin");
        await IdentityTestData.AddUserToRoleAsync(context, user, role);
        context.Localizer.Set("message:validation:duplicatedEntity", "Duplicated {0}");

        ShallNotAllowDuplicateUserRoleRule rule = new ShallNotAllowDuplicateUserRoleRule(
            context.AdministrationDbContextFactory,
            context.Localizer);

        await Assert.ThrowsAsync<DuplicatedEntityException>(
            () => rule.Handle(new CreateUserRoleEvent(user.Id, role.Id), CancellationToken.None));
    }
}
