using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Rules;
using CqrsProject.Core.Identity.UseCases.CreateRoleClaim;
using CqrsProject.Core.Test.Identity;
using CqrsProject.Core.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Identity.Rules;

public class ShallNotAllowDuplicateRoleClaimRuleTest
{
    [Fact(DisplayName = "Should allow a unique role claim")]
    public async Task GivenUniqueRoleClaim_WhenHandled_ThenCompleteWithoutException()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        var role = await IdentityTestData.AddRoleAsync(context, "Admin");
        int initialRoleClaimCount = await dbContext.RoleClaims.CountAsync();

        ShallNotAllowDuplicateRoleClaimRule rule = new ShallNotAllowDuplicateRoleClaimRule(
            context.AdministrationDbContextFactory,
            context.Localizer);

        await rule.Handle(new CreateRoleClaimEvent(role.Id, "permission", "read"), CancellationToken.None);

        Assert.Equal(initialRoleClaimCount, await dbContext.RoleClaims.CountAsync());
    }

    [Fact(DisplayName = "Should reject duplicate role claims")]
    public async Task GivenDuplicatedRoleClaim_WhenHandled_ThenThrowDuplicatedEntityException()
    {
        using CoreTestContext context = new CoreTestContext();
        var role = await IdentityTestData.AddRoleAsync(context, "Admin");
        await IdentityTestData.AddRoleClaimAsync(context, role, "permission", "read");
        context.Localizer.Set("message:validation:duplicatedEntity", "Duplicated {0}");
        context.Localizer.Set("message:validation:combinationValuesAlreadyUse", "The value is duplicated");

        ShallNotAllowDuplicateRoleClaimRule rule = new ShallNotAllowDuplicateRoleClaimRule(
            context.AdministrationDbContextFactory,
            context.Localizer);

        await Assert.ThrowsAsync<DuplicatedEntityException>(
            () => rule.Handle(new CreateRoleClaimEvent(role.Id, "permission", "read"), CancellationToken.None));
    }
}

