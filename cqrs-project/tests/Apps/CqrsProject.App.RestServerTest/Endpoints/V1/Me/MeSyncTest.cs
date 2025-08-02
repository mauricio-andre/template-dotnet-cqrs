using System.Net;
using System.Net.Http.Headers;
using CqrsProject.Common.Consts;
using CqrsProject.Common.Providers.OAuth.Dtos;
using CqrsProject.Common.Providers.OAuth.Interfaces;
using CqrsProject.Commons.Test.Helpers;
using CqrsProject.Core.Data;
using CqrsProject.Core.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace CqrsProject.App.RestServerTest.Endpoints.V1.Me;

public class MeSyncTest
    : IClassFixture<RestServerWebApplicationFactory>,
    IClassFixture<MeSyncTestFake>
{
    private readonly HttpClient _client;
    private readonly RestServerWebApplicationFactory _factory;
    private readonly MeSyncTestFake _meSyncTestFake;
    private const string Route = "/v1/me/sync";

    public MeSyncTest(RestServerWebApplicationFactory factory, MeSyncTestFake meSyncTestFake)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _meSyncTestFake = meSyncTestFake;
    }

    [Fact(DisplayName = "Should fail with unauthorized code when the token is missing")]
    public async Task GivenMeSync_WhenTokenIsMissing_ThenFailWithUnauthorized()
    {
        var response = await _client.PostAsync(Route, null);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Should create a user account when the token is valid and the user does not exist")]
    public async Task GivenMeSync_WhenUserDoesNotExist_ThenCreateLocalUser()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var fakeData = _meSyncTestFake.Generate();

            var mockOAuthService = scope.ServiceProvider.GetRequiredService<IOAuthService>();
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AdministrationDbContext>>();
            var context = dbContextFactory.CreateDbContext();
            var userBeforeRunner = await context.Users
                .FirstOrDefaultAsync(entity => entity.Email == fakeData.Email);

            var expectedUserInfo = new OAuthUserInfoDto(fakeData.Email, fakeData.Username, true);
            var token = JwtHelper.GenerateJwtToken(fakeData.NameIdentifier);

            mockOAuthService.GetUserInfoAsync(token).Returns(expectedUserInfo);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync(Route, null);

            var userAfterRunner = await context.Users
                .FirstOrDefaultAsync(entity => entity.Email == fakeData.Email);

            await mockOAuthService.Received().GetUserInfoAsync(token);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Null(userBeforeRunner);
            Assert.NotNull(userAfterRunner);
            Assert.Equal(expectedUserInfo.PreferredUserName, userAfterRunner.UserName);
            Assert.Equal(expectedUserInfo.Email, userAfterRunner.Email);
        }
    }

    [Fact(DisplayName = "Should update the user account when the user exists")]
    public async Task GivenMeSync_WhenUserExists_ThenUpdateUser()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var fakeData = _meSyncTestFake.Generate();

            var mockOAuthService = scope.ServiceProvider.GetRequiredService<IOAuthService>();
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AdministrationDbContext>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var context = dbContextFactory.CreateDbContext();
            var userBeforeRunner = new User()
            {
                UserName = fakeData.Username,
                Email = fakeData.Email
            };

            await userManager.CreateAsync(userBeforeRunner);

            await userManager.AddLoginAsync(
                userBeforeRunner,
                new UserLoginInfo(
                    AuthenticationDefaults.AuthenticationScheme,
                    fakeData.NameIdentifier,
                    AuthenticationDefaults.DisplayName
                ));

            fakeData.Username = string.Concat("new", fakeData.Username);
            fakeData.Email = string.Concat("new", fakeData.Email);

            var expectedUserInfo = new OAuthUserInfoDto(fakeData.Email, fakeData.Username, true);
            var token = JwtHelper.GenerateJwtToken(fakeData.NameIdentifier);

            mockOAuthService.GetUserInfoAsync(token).Returns(expectedUserInfo);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync(Route, null);

            var userAfterRunner = await context.Users
                .FirstOrDefaultAsync(entity => entity.Email == fakeData.Email);

            await mockOAuthService.Received().GetUserInfoAsync(token);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.NotNull(userBeforeRunner);
            Assert.NotNull(userAfterRunner);
            Assert.Equal(userBeforeRunner.Id, userAfterRunner.Id);
            Assert.NotEqual(userBeforeRunner.Email, userAfterRunner.Email);
            Assert.Equal(userBeforeRunner.UserName, userAfterRunner.UserName);
            Assert.Equal(expectedUserInfo.Email, userAfterRunner.Email);
        }
    }

    [Fact(DisplayName = "Should reactivate the user when it was deleted")]
    public async Task GivenMeSync_WhenUserIsDeleted_ThenReactivateUser()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var fakeData = _meSyncTestFake.Generate();

            var mockOAuthService = scope.ServiceProvider.GetRequiredService<IOAuthService>();
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AdministrationDbContext>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var context = dbContextFactory.CreateDbContext();
            var userBeforeRunner = new User()
            {
                UserName = fakeData.Username,
                Email = fakeData.Email,
                IsDeleted = true
            };

            await userManager.CreateAsync(userBeforeRunner);

            await userManager.AddLoginAsync(
                userBeforeRunner,
                new UserLoginInfo(
                    AuthenticationDefaults.AuthenticationScheme,
                    fakeData.NameIdentifier,
                    AuthenticationDefaults.DisplayName
                ));

            var expectedUserInfo = new OAuthUserInfoDto(fakeData.Email, fakeData.Username, true);
            var token = JwtHelper.GenerateJwtToken(fakeData.NameIdentifier);

            mockOAuthService.GetUserInfoAsync(token).Returns(expectedUserInfo);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync(Route, null);

            var userAfterRunner = await context.Users
                .FirstOrDefaultAsync(entity => entity.Email == fakeData.Email);

            await mockOAuthService.Received().GetUserInfoAsync(token);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.NotNull(userBeforeRunner);
            Assert.NotNull(userAfterRunner);
            Assert.Equal(userBeforeRunner.Id, userAfterRunner.Id);
            Assert.NotEqual(userBeforeRunner.IsDeleted, userAfterRunner.IsDeleted);
            Assert.False(userAfterRunner.IsDeleted);
        }
    }

    [Fact(DisplayName = "Should bind user accounts when an email exists with another login")]
    public async Task GivenMeSync_WhenUserExistsWithOtherLogin_ThenBindLogin()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var fakeData = _meSyncTestFake.Generate();

            var mockOAuthService = scope.ServiceProvider.GetRequiredService<IOAuthService>();
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AdministrationDbContext>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var context = dbContextFactory.CreateDbContext();
            var userBeforeRunner = new User()
            {
                UserName = fakeData.Username,
                Email = fakeData.Email
            };

            await userManager.CreateAsync(userBeforeRunner);

            var userLoginBeforeRunner = await context.UserLogins
                .FirstOrDefaultAsync(entity => entity.ProviderKey == fakeData.NameIdentifier);

            var token = JwtHelper.GenerateJwtToken(fakeData.NameIdentifier);

            mockOAuthService
                .GetUserInfoAsync(token)
                .Returns(new OAuthUserInfoDto(fakeData.Email, fakeData.Username, true));

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync(Route, null);

            var userAfterRunner = await context.Users
                .FirstOrDefaultAsync(entity => entity.Email == fakeData.Email);
            var userLoginAfterRunner = await context.UserLogins
                .FirstOrDefaultAsync(entity => entity.ProviderKey == fakeData.NameIdentifier);

            await mockOAuthService.Received().GetUserInfoAsync(token);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Null(userLoginBeforeRunner);
            Assert.NotNull(userBeforeRunner);
            Assert.NotNull(userAfterRunner);
            Assert.NotNull(userLoginAfterRunner);
            Assert.Equal(userLoginAfterRunner.UserId, userAfterRunner.Id);
            Assert.Equal(userBeforeRunner.Id, userAfterRunner.Id);
            Assert.Equal(userBeforeRunner.UserName, userAfterRunner.UserName);
            Assert.Equal(userBeforeRunner.Email, userAfterRunner.Email);
        }
    }
}
