using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.Test.UserTenants;
using CqrsProject.Core.UserTenants.UseCases.RemoveUserTenant;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.UserTenants.UseCases;

public class RemoveUserTenantTest
{
    [Fact(DisplayName = "Should remove an existing user tenant")]
    public async Task GivenExistingUserTenant_WhenHandled_ThenRemoveItFromDatabase()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        Guid userId = Guid.NewGuid();
        Guid tenantId = Guid.NewGuid();

        await UserTenantsTestData.AddUserAsync(dbContext, userId, "alice");
        await UserTenantsTestData.AddTenantAsync(dbContext, tenantId, "Tenant One");
        await UserTenantsTestData.AddUserTenantAsync(dbContext, userId, tenantId);

        RemoveUserTenantHandler handler = new RemoveUserTenantHandler(
            context.AdministrationDbContextFactory,
            new RemoveUserTenantValidator(),
            context.Localizer);

        await handler.Handle(new RemoveUserTenantCommand(userId, tenantId), CancellationToken.None);

        using AdministrationDbContext verificationDbContext = context.CreateAdministrationDbContext();
        Assert.Empty(await verificationDbContext.UserTenants.ToListAsync());
    }

    [Fact(DisplayName = "Should reject removing a user tenant that does not exist")]
    public async Task GivenMissingUserTenant_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();

        RemoveUserTenantHandler handler = new RemoveUserTenantHandler(
            context.AdministrationDbContextFactory,
            new RemoveUserTenantValidator(),
            context.Localizer);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new RemoveUserTenantCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None));
    }

    [Theory(DisplayName = "Should reject invalid user tenant identifiers before removing")]
    [MemberData(nameof(InvalidCommands))]
    public async Task GivenInvalidIdentifiers_WhenHandled_ThenThrowValidationException(
        Guid userId,
        Guid tenantId)
    {
        using CoreTestContext context = new CoreTestContext();
        RemoveUserTenantHandler handler = new RemoveUserTenantHandler(
            context.AdministrationDbContextFactory,
            new RemoveUserTenantValidator(),
            context.Localizer);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new RemoveUserTenantCommand(userId, tenantId), CancellationToken.None));
    }

    public static IEnumerable<object[]> InvalidCommands()
    {
        yield return new object[] { Guid.Empty, Guid.NewGuid() };
        yield return new object[] { Guid.NewGuid(), Guid.Empty };
    }
}

