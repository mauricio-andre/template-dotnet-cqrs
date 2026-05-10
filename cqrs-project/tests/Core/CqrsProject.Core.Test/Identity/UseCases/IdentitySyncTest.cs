using CqrsProject.Common.Consts;
using CqrsProject.Common.Providers.OAuth.Dtos;
using CqrsProject.Common.Providers.OAuth.Interfaces;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using CqrsProject.Core.Identity.UseCases.IdentitySync;
using CqrsProject.Core.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace CqrsProject.Core.Test.Identity.UseCases;

public class IdentitySyncTest
{
    [Fact(DisplayName = "Should create a local user when there is no linked identity")]
    public async Task GivenUnlinkedOAuthUser_WhenHandled_ThenCreateLocalUserAndLogin()
    {
        using CoreTestContext context = new CoreTestContext();
        IOAuthService oAuthService = Substitute.For<IOAuthService>();
        oAuthService.GetUserInfoAsync("access-token", Arg.Any<CancellationToken>())
            .Returns(new OAuthUserInfoDto("alice@example.com", "alice", true));

        IdentitySyncHandler handler = new IdentitySyncHandler(context.UserManager, oAuthService);

        await handler.Handle(new IdentitySyncCommand("name-id", "access-token"), CancellationToken.None);

        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        User user = await dbContext.Users.SingleAsync();
        Assert.Equal("alice@example.com", user.Email);
        Assert.Equal("alice", user.UserName);
        Assert.False(user.IsDeleted);
        Assert.Single(await context.UserManager.GetLoginsAsync(user));
    }

    [Fact(DisplayName = "Should bind an existing user by email when login is missing")]
    public async Task GivenMatchingEmail_WhenHandled_ThenBindLogin()
    {
        using CoreTestContext context = new CoreTestContext();
        User existingUser = await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");
        IOAuthService oAuthService = Substitute.For<IOAuthService>();
        oAuthService.GetUserInfoAsync("access-token", Arg.Any<CancellationToken>())
            .Returns(new OAuthUserInfoDto("alice@example.com", "alice-new", true));

        IdentitySyncHandler handler = new IdentitySyncHandler(context.UserManager, oAuthService);

        await handler.Handle(new IdentitySyncCommand("name-id", "access-token"), CancellationToken.None);

        Assert.Single(await context.UserManager.GetLoginsAsync(existingUser));
    }

    [Fact(DisplayName = "Should update a linked local user when the login already exists")]
    public async Task GivenLinkedUser_WhenHandled_ThenUpdateLocalUser()
    {
        using CoreTestContext context = new CoreTestContext();
        User existingUser = await IdentityTestData.AddUserAsync(context, userName: "alice", email: "alice@example.com");
        await IdentityTestData.AddLoginAsync(context, existingUser, "name-id");

        IOAuthService oAuthService = Substitute.For<IOAuthService>();
        oAuthService.GetUserInfoAsync("access-token", Arg.Any<CancellationToken>())
            .Returns(new OAuthUserInfoDto("alice-new@example.com", "alice-new", false));

        IdentitySyncHandler handler = new IdentitySyncHandler(context.UserManager, oAuthService);

        await handler.Handle(new IdentitySyncCommand("name-id", "access-token"), CancellationToken.None);

        using AdministrationDbContext dbContext = context.CreateAdministrationDbContext();
        User user = await dbContext.Users.SingleAsync();
        Assert.Equal("alice-new@example.com", user.Email);
        Assert.False(user.EmailConfirmed);
        Assert.NotNull(user.LastModificationTime);
    }
}
