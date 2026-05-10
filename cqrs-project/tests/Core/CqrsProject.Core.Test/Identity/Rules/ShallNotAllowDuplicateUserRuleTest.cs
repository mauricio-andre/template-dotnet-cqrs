using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.Rules;
using CqrsProject.Core.Identity.UseCases.CreateUser;
using CqrsProject.Core.Identity.UseCases.UpdateUser;
using CqrsProject.Core.Test.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CqrsProject.Core.Test.Identity.Rules;

public class ShallNotAllowDuplicateUserRuleTest
{
    [Theory(DisplayName = "Should allow unique users for every supported event")]
    [MemberData(nameof(GetUniqueUserCases))]
    public async Task GivenUniqueUser_WhenHandled_ThenCompleteWithoutException(
        Func<CoreTestContext, Task> arrange,
        Func<ShallNotAllowDuplicateUserRule, Task> act)
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        await arrange(context);
        int initialUserCount = await dbContext.Users.CountAsync();
        context.Localizer.Set("message:validation:duplicatedEntity", "Duplicated {0}");
        context.Localizer.Set("message:validation:valueAlreadyUse", "The value {0} is already in use");

        ShallNotAllowDuplicateUserRule rule = new ShallNotAllowDuplicateUserRule(
            context.AdministrationDbContextFactory,
            context.Localizer,
            context.UserManager);

        await act(rule);

        Assert.Equal(initialUserCount, await dbContext.Users.CountAsync());
    }

    [Theory(DisplayName = "Should reject duplicated users for every supported event")]
    [MemberData(nameof(GetDuplicatedUserCases))]
    public async Task GivenDuplicatedUser_WhenHandled_ThenThrowDuplicatedEntityException(
        Func<CoreTestContext, Task> arrange,
        Func<ShallNotAllowDuplicateUserRule, Task> act)
    {
        using CoreTestContext context = new CoreTestContext();
        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        await arrange(context);
        context.Localizer.Set("message:validation:duplicatedEntity", "Duplicated {0}");
        context.Localizer.Set("message:validation:valueAlreadyUse", "The value {0} is already in use");

        ShallNotAllowDuplicateUserRule rule = new ShallNotAllowDuplicateUserRule(
            context.AdministrationDbContextFactory,
            context.Localizer,
            context.UserManager);

        DuplicatedEntityException exception = await Assert.ThrowsAsync<DuplicatedEntityException>(
            () => act(rule));

        Assert.Equal("Duplicated User", exception.Message);
    }

    public static TheoryData<Func<CoreTestContext, Task>, Func<ShallNotAllowDuplicateUserRule, Task>> GetUniqueUserCases => new()
    {
        {
            async context => await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com"),
            rule => rule.Handle(new CreateUserEvent("bob", "bob@example.com"), CancellationToken.None)
        },
        {
            async context =>
            {
                await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");
            },
            rule => rule.Handle(new UpdateUserEvent(Guid.NewGuid(), "bob", "bob@example.com"), CancellationToken.None)
        }
    };

    public static TheoryData<Func<CoreTestContext, Task>, Func<ShallNotAllowDuplicateUserRule, Task>> GetDuplicatedUserCases => new()
    {
        {
            async context => await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com"),
            rule => rule.Handle(new CreateUserEvent("alice", "alice-two@example.com"), CancellationToken.None)
        },
        {
            async context => await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com"),
            rule => rule.Handle(new CreateUserEvent("alice-two", "alice@example.com"), CancellationToken.None)
        },
        {
            async context =>
            {
                await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");
                await IdentityTestData.AddUserAsync(context, userName: "bob", email: "bob@example.com");
            },
            rule => rule.Handle(new UpdateUserEvent(Guid.NewGuid(), "alice", "alice-two@example.com"), CancellationToken.None)
        },
        {
            async context =>
            {
                await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");
                await IdentityTestData.AddUserAsync(context, userName: "bob", email: "bob@example.com");
            },
            rule => rule.Handle(new UpdateUserEvent(Guid.NewGuid(), "alice-two", "alice@example.com"), CancellationToken.None)
        }
    };
}
