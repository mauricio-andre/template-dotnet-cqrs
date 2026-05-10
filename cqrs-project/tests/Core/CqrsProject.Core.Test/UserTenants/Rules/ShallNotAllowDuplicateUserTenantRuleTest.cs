using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.UserTenants.Rules;
using CqrsProject.Core.UserTenants.UseCases.CreateUserTenant;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.UserTenants.Rules;

public class ShallNotAllowDuplicateUserTenantRuleTest
{
    [Fact(DisplayName = "Should allow a unique user tenant")]
    public async Task GivenUniqueUserTenant_WhenHandled_ThenCompleteWithoutException()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        await AdministrationTestData.AddUserAsync(dbContext, Guid.NewGuid(), "alice");
        await AdministrationTestData.AddTenantAsync(dbContext, Guid.NewGuid(), "Tenant One");

        ShallNotAllowDuplicateUserTenantRule rule = new ShallNotAllowDuplicateUserTenantRule(
            context.AdministrationDbContextFactory,
            context.Localizer);

        await rule.Handle(new CreateUserTenantEvent(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        Assert.True(true);
    }

    [Fact(DisplayName = "Should reject duplicated user tenants")]
    public async Task GivenDuplicatedUserTenant_WhenHandled_ThenThrowDuplicatedEntityException()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        Guid userId = Guid.NewGuid();
        Guid tenantId = Guid.NewGuid();

        await AdministrationTestData.AddUserAsync(dbContext, userId, "alice");
        await AdministrationTestData.AddTenantAsync(dbContext, tenantId, "Tenant One");
        await AdministrationTestData.AddUserTenantAsync(dbContext, userId, tenantId);

        context.Localizer.Set("message:validation:duplicatedEntity", "Duplicated {0}");

        ShallNotAllowDuplicateUserTenantRule rule = new ShallNotAllowDuplicateUserTenantRule(
            context.AdministrationDbContextFactory,
            context.Localizer);

        DuplicatedEntityException exception = await Assert.ThrowsAsync<DuplicatedEntityException>(
            () => rule.Handle(new CreateUserTenantEvent(userId, tenantId), CancellationToken.None));

        Assert.Equal("Duplicated UserTenant", exception.Message);
    }
}
