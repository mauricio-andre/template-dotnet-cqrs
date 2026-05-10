using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Rules;
using CqrsProject.Core.Tenants.UseCases.CreateTenant;
using CqrsProject.Core.Tenants.UseCases.UpdateTenant;

namespace CqrsProject.Core.Test.Tenants.Rules;

public class ShallNotAllowDuplicateTenantRuleTest
{
    [Fact(DisplayName = "Should allow a unique tenant name")]
    public async Task GivenUniqueTenantName_WhenHandled_ThenCompleteWithoutException()
    {
        using CoreTestContext context = new CoreTestContext();
        ShallNotAllowDuplicateTenantRule rule = new ShallNotAllowDuplicateTenantRule(
            context.AdministrationDbContextFactory,
            context.Localizer);

        await rule.Handle(new CreateTenantEvent("Tenant One"), CancellationToken.None);

        Assert.True(true);
    }

    [Fact(DisplayName = "Should reject duplicated tenant names on create")]
    public async Task GivenDuplicatedTenantNameOnCreate_WhenHandled_ThenThrowDuplicatedEntityException()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        await AdministrationTestData.AddTenantAsync(dbContext, name: "Tenant One");
        context.Localizer.Set("message:validation:duplicatedEntity", "Duplicated {0}");
        context.Localizer.Set("message:validation:valueAlreadyUse", "The value {0} is already in use");

        ShallNotAllowDuplicateTenantRule rule = new ShallNotAllowDuplicateTenantRule(
            context.AdministrationDbContextFactory,
            context.Localizer);

        DuplicatedEntityException exception = await Assert.ThrowsAsync<DuplicatedEntityException>(
            () => rule.Handle(new CreateTenantEvent("tenant one"), CancellationToken.None));

        Assert.Equal("Duplicated Tenant", exception.Message);
        Assert.True(exception.Errors.ContainsKey(nameof(Tenant.Name)));
        Assert.Equal("The value tenant one is already in use", exception.Errors[nameof(Tenant.Name)].Single());
    }

    [Fact(DisplayName = "Should reject duplicated tenant names on update when another tenant exists")]
    public async Task GivenDuplicatedTenantNameOnUpdate_WhenHandled_ThenThrowDuplicatedEntityException()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        Tenant tenant = await AdministrationTestData.AddTenantAsync(dbContext, name: "Tenant One");
        await AdministrationTestData.AddTenantAsync(dbContext, name: "Tenant Two");
        context.Localizer.Set("message:validation:duplicatedEntity", "Duplicated {0}");
        context.Localizer.Set("message:validation:valueAlreadyUse", "The value {0} is already in use");

        ShallNotAllowDuplicateTenantRule rule = new ShallNotAllowDuplicateTenantRule(
            context.AdministrationDbContextFactory,
            context.Localizer);

        DuplicatedEntityException exception = await Assert.ThrowsAsync<DuplicatedEntityException>(
            () => rule.Handle(new UpdateTenantEvent(tenant.Id, "Tenant Two"), CancellationToken.None));

        Assert.Equal("Duplicated Tenant", exception.Message);
        Assert.True(exception.Errors.ContainsKey(nameof(Tenant.Name)));
    }
}
