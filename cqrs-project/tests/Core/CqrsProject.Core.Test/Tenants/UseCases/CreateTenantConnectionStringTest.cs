using CqrsProject.Core.Data;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Tenants.Responses;
using CqrsProject.Core.Tenants.UseCases.CreateTenantConnectionString;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace CqrsProject.Core.Test.Tenants.UseCases;

public class CreateTenantConnectionStringTest
{
    [Fact(DisplayName = "Should create a tenant connection string and include it in the provider")]
    public async Task GivenValidCommand_WhenHandled_ThenPersistConnectionStringAndIncludeIt()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        Guid tenantId = Guid.NewGuid();

        await AdministrationTestData.AddTenantAsync(dbContext, tenantId, "Tenant One");

        CreateTenantConnectionStringHandler handler = new CreateTenantConnectionStringHandler(
            context.AdministrationDbContextFactory,
            new CreateTenantConnectionStringValidator(),
            context.Mediator,
            context.TenantConnectionProvider);

        TenantConnectionStringResponse response = await handler.Handle(
            new CreateTenantConnectionStringCommand("Default", "Host=localhost;", tenantId),
            CancellationToken.None);

        Assert.True(response.Id != Guid.Empty);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("Default", response.ConnectionName);
        Assert.Equal("Host=localhost;", response.KeyName);
        Assert.Equal(1, await dbContext.TenantConnectionStrings.CountAsync());

        await context.Mediator.Received(1).Publish(
            Arg.Is<CreateTenantConnectionStringEvent>(notification =>
                notification.TenantId == tenantId
                && notification.ConnectionName == "Default"),
            Arg.Any<CancellationToken>());

        await context.TenantConnectionProvider.Received(1).IncludeConnectionStringAsync(
            tenantId,
            "Default",
            "Host=localhost;");
    }

    [Theory(DisplayName = "Should reject invalid tenant connection string commands")]
    [MemberData(nameof(InvalidCommands))]
    public async Task GivenInvalidCommand_WhenHandled_ThenThrowValidationException(
        Guid tenantId,
        string connectionName,
        string keyName)
    {
        using CoreTestContext context = new CoreTestContext();
        CreateTenantConnectionStringHandler handler = new CreateTenantConnectionStringHandler(
            context.AdministrationDbContextFactory,
            new CreateTenantConnectionStringValidator(),
            context.Mediator,
            context.TenantConnectionProvider);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new CreateTenantConnectionStringCommand(connectionName, keyName, tenantId), CancellationToken.None));
    }

    public static IEnumerable<object[]> InvalidCommands()
    {
        yield return new object[] { Guid.Empty, "Default", "Host=localhost;" };
        yield return new object[] { Guid.NewGuid(), string.Empty, "Host=localhost;" };
        yield return new object[] { Guid.NewGuid(), "Default", string.Empty };
    }
}
