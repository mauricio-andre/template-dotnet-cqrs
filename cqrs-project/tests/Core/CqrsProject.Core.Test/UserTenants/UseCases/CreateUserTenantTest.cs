using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Test.Infrastructure;
using CqrsProject.Core.UserTenants.Entities;
using CqrsProject.Core.UserTenants.UseCases.CreateUserTenant;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace CqrsProject.Core.Test.UserTenants.UseCases;

public class CreateUserTenantTest
{
    [Fact(DisplayName = "Should create a user tenant and publish the creation event")]
    public async Task GivenValidCommand_WhenHandled_ThenPersistUserTenantAndPublishEvent()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        Guid userId = Guid.NewGuid();
        Guid tenantId = Guid.NewGuid();
        Guid creatorId = Guid.NewGuid();

        await AdministrationTestData.AddUserAsync(dbContext, userId, "alice");
        await AdministrationTestData.AddTenantAsync(dbContext, tenantId, "Tenant One");
        context.CurrentIdentity.GetLocalIdentityId().Returns(creatorId);

        CreateUserTenantHandler handler = new CreateUserTenantHandler(
            context.AdministrationDbContextFactory,
            new CreateUserTenantValidator(),
            context.Mediator,
            context.Localizer,
            context.CurrentIdentity);

        await handler.Handle(new CreateUserTenantCommand(userId, tenantId), CancellationToken.None);

        using AdministrationDbContext verificationDbContext = context.CreateAdministrationDbContext();
        UserTenant userTenant = await verificationDbContext.UserTenants.SingleAsync();

        Assert.Equal(userId, userTenant.UserId);
        Assert.Equal(tenantId, userTenant.TenantId);
        Assert.Equal(creatorId, userTenant.CreatorId);
        Assert.True(userTenant.CreationTime > DateTimeOffset.MinValue);

        await context.Mediator.Received(1).Publish(
            Arg.Is<CreateUserTenantEvent>(eventNotification =>
                eventNotification.UserId == userId
                && eventNotification.TenantId == tenantId),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should reject a user tenant when the user does not exist")]
    public async Task GivenMissingUser_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        Guid userId = Guid.NewGuid();
        Guid tenantId = Guid.NewGuid();

        await AdministrationTestData.AddTenantAsync(dbContext, tenantId, "Tenant One");

        CreateUserTenantHandler handler = new CreateUserTenantHandler(
            context.AdministrationDbContextFactory,
            new CreateUserTenantValidator(),
            context.Mediator,
            context.Localizer,
            context.CurrentIdentity);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new CreateUserTenantCommand(userId, tenantId), CancellationToken.None));
    }

    [Fact(DisplayName = "Should reject a user tenant when the tenant does not exist")]
    public async Task GivenMissingTenant_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();

        Guid userId = Guid.NewGuid();
        Guid tenantId = Guid.NewGuid();

        await AdministrationTestData.AddUserAsync(dbContext, userId, "alice");

        CreateUserTenantHandler handler = new CreateUserTenantHandler(
            context.AdministrationDbContextFactory,
            new CreateUserTenantValidator(),
            context.Mediator,
            context.Localizer,
            context.CurrentIdentity);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new CreateUserTenantCommand(userId, tenantId), CancellationToken.None));
    }

    [Theory(DisplayName = "Should reject invalid user tenant identifiers")]
    [MemberData(nameof(InvalidCommands))]
    public async Task GivenInvalidIdentifiers_WhenHandled_ThenThrowValidationException(
        Guid userId,
        Guid tenantId)
    {
        using CoreTestContext context = new CoreTestContext();
        CreateUserTenantHandler handler = new CreateUserTenantHandler(
            context.AdministrationDbContextFactory,
            new CreateUserTenantValidator(),
            context.Mediator,
            context.Localizer,
            context.CurrentIdentity);

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.Handle(new CreateUserTenantCommand(userId, tenantId), CancellationToken.None));
    }

    public static IEnumerable<object[]> InvalidCommands()
    {
        yield return new object[] { Guid.Empty, Guid.NewGuid() };
        yield return new object[] { Guid.NewGuid(), Guid.Empty };
    }
}
