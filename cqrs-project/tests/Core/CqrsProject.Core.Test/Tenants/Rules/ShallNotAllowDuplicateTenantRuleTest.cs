using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Rules;
using CqrsProject.Core.Tenants.UseCases.CreateTenant;
using CqrsProject.Core.Tenants.UseCases.UpdateTenant;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Test.Tenants;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Tenants.Rules;

public class ShallNotAllowDuplicateTenantRuleTest
{
    [Theory(DisplayName = "Should allow unique tenant names for every supported event")]
    [MemberData(nameof(GetUniqueTenantCases))]
    public async Task GivenUniqueTenantName_WhenHandled_ThenCompleteWithoutException(
        Func<CoreTestContext, Task> arrange,
        Func<ShallNotAllowDuplicateTenantRule, Task> act)
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        await arrange(context);
        int initialTenantCount = await dbContext.Tenants.CountAsync();
        ShallNotAllowDuplicateTenantRule rule = new ShallNotAllowDuplicateTenantRule(
            context.AdministrationDbContextFactory,
            context.Localizer);

        await act(rule);

        Assert.Equal(initialTenantCount, await dbContext.Tenants.CountAsync());
    }

    [Theory(DisplayName = "Should reject duplicated tenant names for every supported event")]
    [MemberData(nameof(GetDuplicatedTenantCases))]
    public async Task GivenDuplicatedTenantName_WhenHandled_ThenThrowDuplicatedEntityException(
        Func<CoreTestContext, Task> arrange,
        Func<ShallNotAllowDuplicateTenantRule, Task> act)
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        await arrange(context);
        context.Localizer.Set("message:validation:duplicatedEntity", "Duplicated {0}");
        context.Localizer.Set("message:validation:valueAlreadyUse", "The value {0} is already in use");

        ShallNotAllowDuplicateTenantRule rule = new ShallNotAllowDuplicateTenantRule(
            context.AdministrationDbContextFactory,
            context.Localizer);

        DuplicatedEntityException exception = await Assert.ThrowsAsync<DuplicatedEntityException>(
            () => act(rule));

        Assert.Equal("Duplicated Tenant", exception.Message);
        Assert.True(exception.Errors.ContainsKey(nameof(Tenant.Name)));
    }

    public static TheoryData<Func<CoreTestContext, Task>, Func<ShallNotAllowDuplicateTenantRule, Task>> GetUniqueTenantCases => new()
    {
        {
            async context =>
            {
                using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
                await TenantsTestData.AddTenantAsync(dbContext, name: "Tenant One");
            },
            rule => rule.Handle(new CreateTenantEvent("Tenant Two"), CancellationToken.None)
        },
        {
            async context =>
            {
                using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
                await TenantsTestData.AddTenantAsync(dbContext, name: "Tenant One");
            },
            rule => rule.Handle(new UpdateTenantEvent(Guid.NewGuid(), "Tenant Two"), CancellationToken.None)
        }
    };

    public static TheoryData<Func<CoreTestContext, Task>, Func<ShallNotAllowDuplicateTenantRule, Task>> GetDuplicatedTenantCases => new()
    {
        {
            async context =>
            {
                using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
                await TenantsTestData.AddTenantAsync(dbContext, name: "Tenant One");
            },
            rule => rule.Handle(new CreateTenantEvent("tenant one"), CancellationToken.None)
        },
        {
            async context =>
            {
                using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
                await TenantsTestData.AddTenantAsync(dbContext, name: "Tenant One");
                await TenantsTestData.AddTenantAsync(dbContext, name: "Tenant Two");
            },
            rule => rule.Handle(new UpdateTenantEvent(Guid.NewGuid(), "Tenant Two"), CancellationToken.None)
        }
    };
}

