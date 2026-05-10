using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.UseCases.RemoveTenantConnectionString;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace CqrsProject.Core.Test.Tenants.UseCases;

public class RemoveTenantConnectionStringTest
{
    [Fact(DisplayName = "Should remove a tenant connection string and invalidate it in the provider")]
    public async Task GivenExistingConnectionString_WhenHandled_ThenRemoveItAndInvalidateIt()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        Guid tenantId = Guid.NewGuid();
        await AdministrationTestData.AddTenantAsync(dbContext, tenantId, "Tenant One");
        TenantConnectionString connectionString = await AdministrationTestData.AddTenantConnectionStringAsync(
            dbContext,
            tenantId,
            "Default",
            "Host=localhost;");

        RemoveTenantConnectionStringHandler handler = new RemoveTenantConnectionStringHandler(
            context.AdministrationDbContextFactory,
            new RemoveTenantConnectionStringValidator(),
            context.Localizer,
            context.TenantConnectionProvider);

        await handler.Handle(new RemoveTenantConnectionStringCommand(connectionString.Id, tenantId), CancellationToken.None);

        Assert.Empty(await dbContext.TenantConnectionStrings.ToListAsync());

        context.TenantConnectionProvider.Received(1).InvalidateConnectionString(tenantId, "Default");
    }

    [Fact(DisplayName = "Should reject removing a missing tenant connection string")]
    public async Task GivenMissingConnectionString_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();

        RemoveTenantConnectionStringHandler handler = new RemoveTenantConnectionStringHandler(
            context.AdministrationDbContextFactory,
            new RemoveTenantConnectionStringValidator(),
            context.Localizer,
            context.TenantConnectionProvider);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new RemoveTenantConnectionStringCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }

    [Fact(DisplayName = "Should reject invalid tenant connection string identifiers")]
    public async Task GivenInvalidIdentifiers_WhenHandled_ThenThrowValidationException()
    {
        using CoreTestContext context = new CoreTestContext();

        RemoveTenantConnectionStringHandler handler = new RemoveTenantConnectionStringHandler(
            context.AdministrationDbContextFactory,
            new RemoveTenantConnectionStringValidator(),
            context.Localizer,
            context.TenantConnectionProvider);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new RemoveTenantConnectionStringCommand(Guid.Empty, Guid.Empty), CancellationToken.None));
    }
}
