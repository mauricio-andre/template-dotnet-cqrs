using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Responses;
using CqrsProject.Core.Tenants.UseCases.GetTenantConnectionString;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Test.Tenants;
using FluentValidation;

namespace CqrsProject.Core.Test.Tenants.UseCases;

public class GetTenantConnectionStringTest
{
    [Fact(DisplayName = "Should return a tenant connection string when it exists")]
    public async Task GivenExistingConnectionString_WhenHandled_ThenReturnIt()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        Guid tenantId = Guid.NewGuid();
        await TenantsTestData.AddTenantAsync(dbContext, tenantId, "Tenant One");
        TenantConnectionString connectionString = await TenantsTestData.AddTenantConnectionStringAsync(
            dbContext,
            tenantId,
            "Default",
            "Host=localhost;");

        GetTenantConnectionStringHandler handler = new GetTenantConnectionStringHandler(
            context.AdministrationDbContextFactory,
            new GetTenantConnectionStringValidator(),
            context.Localizer);

        TenantConnectionStringResponse response = await handler.Handle(
            new GetTenantConnectionStringQuery(tenantId, connectionString.Id),
            CancellationToken.None);

        Assert.Equal(connectionString.Id, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("Default", response.ConnectionName);
        Assert.Equal("Host=localhost;", response.KeyName);
    }

    [Fact(DisplayName = "Should reject a missing tenant connection string")]
    public async Task GivenMissingConnectionString_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();

        GetTenantConnectionStringHandler handler = new GetTenantConnectionStringHandler(
            context.AdministrationDbContextFactory,
            new GetTenantConnectionStringValidator(),
            context.Localizer);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new GetTenantConnectionStringQuery(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }
}

