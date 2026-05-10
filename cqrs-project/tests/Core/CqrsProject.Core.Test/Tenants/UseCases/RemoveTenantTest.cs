using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Tenants.UseCases.RemoveTenant;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Tenants.UseCases;

public class RemoveTenantTest
{
    [Fact(DisplayName = "Should mark a tenant as deleted")]
    public async Task GivenExistingTenant_WhenHandled_ThenMarkItAsDeleted()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        var tenant = await AdministrationTestData.AddTenantAsync(dbContext, name: "Tenant One");

        RemoveTenantHandler handler = new RemoveTenantHandler(
            context.AdministrationDbContextFactory,
            new RemoveTenantValidator(),
            context.Localizer);

        await handler.Handle(new RemoveTenantCommand(tenant.Id), CancellationToken.None);

        Assert.True(await dbContext.Tenants.Where(item => item.Id == tenant.Id).Select(item => item.IsDeleted).SingleAsync());
    }

    [Fact(DisplayName = "Should reject removing a tenant that does not exist")]
    public async Task GivenMissingTenant_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        RemoveTenantHandler handler = new RemoveTenantHandler(
            context.AdministrationDbContextFactory,
            new RemoveTenantValidator(),
            context.Localizer);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new RemoveTenantCommand(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact(DisplayName = "Should reject invalid tenant identifiers")]
    public async Task GivenInvalidIdentifier_WhenHandled_ThenThrowValidationException()
    {
        using CoreTestContext context = new CoreTestContext();
        RemoveTenantHandler handler = new RemoveTenantHandler(
            context.AdministrationDbContextFactory,
            new RemoveTenantValidator(),
            context.Localizer);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new RemoveTenantCommand(Guid.Empty), CancellationToken.None));
    }
}
