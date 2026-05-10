using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Rules;
using CqrsProject.Core.Tenants.UseCases.CreateTenantConnectionString;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Tenants.Rules;

public class ShallNotAllowDuplicateTenantConnectionStringRuleTest
{
    [Fact(DisplayName = "Should allow a unique tenant connection string")]
    public async Task GivenUniqueConnectionString_WhenHandled_ThenCompleteWithoutException()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        Guid tenantId = Guid.NewGuid();
        await AdministrationTestData.AddTenantAsync(dbContext, tenantId, "Tenant One");
        ShallNotAllowDuplicateTenantConnectionStringRule rule = new ShallNotAllowDuplicateTenantConnectionStringRule(
            context.AdministrationDbContextFactory,
            context.Localizer);

        await rule.Handle(new CreateTenantConnectionStringEvent(tenantId, "Backup"), CancellationToken.None);

        Assert.Empty(await dbContext.TenantConnectionStrings.ToListAsync());
    }

    [Fact(DisplayName = "Should reject duplicated tenant connection strings")]
    public async Task GivenDuplicatedConnectionString_WhenHandled_ThenThrowDuplicatedEntityException()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        Guid tenantId = Guid.NewGuid();
        await AdministrationTestData.AddTenantAsync(dbContext, tenantId, "Tenant One");
        await AdministrationTestData.AddTenantConnectionStringAsync(dbContext, tenantId, "Default", "Host=localhost;");
        context.Localizer.Set("message:validation:duplicatedEntity", "Duplicated {0}");
        context.Localizer.Set("message:validation:valueAlreadyUse", "The value {0} is already in use");

        ShallNotAllowDuplicateTenantConnectionStringRule rule = new ShallNotAllowDuplicateTenantConnectionStringRule(
            context.AdministrationDbContextFactory,
            context.Localizer);

        DuplicatedEntityException exception = await Assert.ThrowsAsync<DuplicatedEntityException>(
            () => rule.Handle(new CreateTenantConnectionStringEvent(tenantId, "default"), CancellationToken.None));

        Assert.Equal("Duplicated TenantConnectionString", exception.Message);
        Assert.True(exception.Errors.ContainsKey(nameof(TenantConnectionString.ConnectionName)));
        Assert.Equal("The value default is already in use", exception.Errors[nameof(TenantConnectionString.ConnectionName)].Single());
    }
}
