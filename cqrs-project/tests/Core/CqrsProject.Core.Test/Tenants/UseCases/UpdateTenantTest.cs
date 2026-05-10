using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Responses;
using CqrsProject.Core.Tenants.UseCases.UpdateTenant;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Test.Tenants;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace CqrsProject.Core.Test.Tenants.UseCases;

public class UpdateTenantTest
{
    [Fact(DisplayName = "Should update a tenant name and publish the update event")]
    public async Task GivenExistingTenant_WhenHandled_ThenUpdateTenantAndPublishEvent()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        Tenant tenant = await TenantsTestData.AddTenantAsync(dbContext, name: "Tenant One");

        UpdateTenantHandler handler = new UpdateTenantHandler(
            context.AdministrationDbContextFactory,
            new UpdateTenantValidator(),
            context.Mediator,
            context.Localizer);

        TenantResponse response = await handler.Handle(
            new UpdateTenantCommand(tenant.Id, "Tenant Two"),
            CancellationToken.None);

        Assert.Equal(tenant.Id, response.Id);
        Assert.Equal("Tenant Two", response.Nome);
        Assert.False(response.IsDeleted);
        Assert.Equal("Tenant Two", await dbContext.Tenants.Select(item => item.Name).SingleAsync());

        await context.Mediator.Received(1).Publish(
            Arg.Is<UpdateTenantEvent>(notification =>
                notification.Id == tenant.Id
                && notification.Name == "Tenant Two"),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should reject updating a tenant that does not exist")]
    public async Task GivenMissingTenant_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        UpdateTenantHandler handler = new UpdateTenantHandler(
            context.AdministrationDbContextFactory,
            new UpdateTenantValidator(),
            context.Mediator,
            context.Localizer);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new UpdateTenantCommand(Guid.NewGuid(), "Tenant Two"), CancellationToken.None));
    }

    [Theory(DisplayName = "Should reject invalid update tenant commands")]
    [MemberData(nameof(InvalidCommands))]
    public async Task GivenInvalidCommand_WhenHandled_ThenThrowValidationException(
        Guid id,
        string name)
    {
        using CoreTestContext context = new CoreTestContext();
        UpdateTenantHandler handler = new UpdateTenantHandler(
            context.AdministrationDbContextFactory,
            new UpdateTenantValidator(),
            context.Mediator,
            context.Localizer);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new UpdateTenantCommand(id, name), CancellationToken.None));
    }

    public static IEnumerable<object[]> InvalidCommands()
    {
        yield return new object[] { Guid.Empty, "Tenant Two" };
        yield return new object[] { Guid.NewGuid(), string.Empty };
    }
}

