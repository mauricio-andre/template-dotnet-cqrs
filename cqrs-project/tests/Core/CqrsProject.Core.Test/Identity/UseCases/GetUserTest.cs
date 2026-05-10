using CqrsProject.Common.Exceptions;
using CqrsProject.Core.Identity.UseCases.GetUser;
using CqrsProject.Core.Test.Infrastructure;
using FluentValidation;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class GetUserTest
{
    [Fact(DisplayName = "Should return a user when it exists")]
    public async Task GivenExistingUser_WhenHandled_ThenReturnUser()
    {
        using CoreTestContext context = new CoreTestContext();
        var user = await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");

        GetUserHandler handler = new GetUserHandler(
            new GetUserValidator(),
            context.Localizer,
            context.UserManager);

        var response = await handler.Handle(new GetUserQuery(user.Id), CancellationToken.None);

        Assert.Equal(user.Id, response.Id);
        Assert.Equal("alice", response.UserName);
    }

    [Fact(DisplayName = "Should reject missing users")]
    public async Task GivenMissingUser_WhenHandled_ThenThrowEntityNotFoundException()
    {
        using CoreTestContext context = new CoreTestContext();
        GetUserHandler handler = new GetUserHandler(
            new GetUserValidator(),
            context.Localizer,
            context.UserManager);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => handler.Handle(new GetUserQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
