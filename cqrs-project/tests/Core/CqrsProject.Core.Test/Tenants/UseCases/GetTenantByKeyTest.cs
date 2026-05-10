using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Responses;
using CqrsProject.Core.Tenants.UseCases.GetTenantByKey;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Test.Tenants;
using FluentValidation;

namespace CqrsProject.Core.Test.Tenants.UseCases;

public class GetTenantByKeyTest
{
    [Fact(DisplayName = "Should return a tenant when the key exists")]
    public async Task GivenExistingTenant_WhenHandled_ThenReturnTenant()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        var tenant = await TenantsTestData.AddTenantAsync(dbContext, name: "Tenant One");

        GetTenantByKeyHandler handler = new GetTenantByKeyHandler(
            context.AdministrationDbContextFactory,
            new GetTenantByKeyValidator(),
            context.Localizer);

        var response = await handler.Handle(new GetTenantByKeyQuery(tenant.Id), CancellationToken.None);

        Assert.Equal(tenant.Id, response.Id);
        Assert.Equal("Tenant One", response.Nome);
        Assert.False(response.IsDeleted);
    }

    [Fact(DisplayName = "Should reject a tenant key that does not exist")]
    public async Task GivenMissingTenant_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        GetTenantByKeyHandler handler = new GetTenantByKeyHandler(
            context.AdministrationDbContextFactory,
            new GetTenantByKeyValidator(),
            context.Localizer);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new GetTenantByKeyQuery(Guid.NewGuid()), CancellationToken.None));
    }
}

