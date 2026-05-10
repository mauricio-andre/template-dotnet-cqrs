using CqrsProject.Core.Data;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.Tenants.Responses;
using CqrsProject.Core.Tenants.UseCases.CreateTenant;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace CqrsProject.Core.Test.Tenants.UseCases;

public class CreateTenantTest
{
    [Fact(DisplayName = "Should create a tenant and publish the creation event")]
    public async Task GivenValidCommand_WhenHandled_ThenPersistTenantAndPublishEvent()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        CreateTenantHandler handler = new CreateTenantHandler(
            context.AdministrationDbContextFactory,
            new CreateTenantValidator(),
            context.Mediator);

        TenantResponse response = await handler.Handle(new CreateTenantCommand("Tenant One"), CancellationToken.None);

        Assert.True(response.Id != Guid.Empty);
        Assert.Equal("Tenant One", response.Nome);
        Assert.False(response.IsDeleted);
        Assert.Equal(1, await dbContext.Tenants.CountAsync());
        Assert.Equal("Tenant One", await dbContext.Tenants.Select(tenant => tenant.Name).SingleAsync());

        await context.Mediator.Received(1).Publish(
            Arg.Is<CreateTenantEvent>(notification => notification.Name == "Tenant One"),
            Arg.Any<CancellationToken>());
    }

    [Theory(DisplayName = "Should reject invalid tenant names")]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GivenInvalidName_WhenHandled_ThenThrowValidationException(string name)
    {
        using CoreTestContext context = new CoreTestContext();
        CreateTenantHandler handler = new CreateTenantHandler(
            context.AdministrationDbContextFactory,
            new CreateTenantValidator(),
            context.Mediator);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new CreateTenantCommand(name), CancellationToken.None));
    }

    [Fact(DisplayName = "Should reject tenant names above the maximum length")]
    public async Task GivenNameAboveMaxLength_WhenHandled_ThenThrowValidationException()
    {
        using CoreTestContext context = new CoreTestContext();
        CreateTenantHandler handler = new CreateTenantHandler(
            context.AdministrationDbContextFactory,
            new CreateTenantValidator(),
            context.Mediator);

        string name = new string('a', TenantConstrains.NameMaxLength + 1);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new CreateTenantCommand(name), CancellationToken.None));
    }
}
