using System.Net;
using System.Net.Http.Headers;
using CqrsProject.Common.Consts;
using CqrsProject.Commons.Test.Helpers;
using CqrsProject.Core.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.App.RestServerTest.Middlewares;

public class TenantMiddlewareTest : IClassFixture<RestServerWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly RestServerWebApplicationFactory _factory;
    private const string Route = "/v1/me/tenants";

    public TenantMiddlewareTest(RestServerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact(DisplayName = "Should fail on any route when the local user does not exist and the tenant exists in the request header")]
    public async Task GivenAnyRoute_WhenLocalUserDoesNotExist_ThenFailWithForbidden()
    {
        var token = JwtHelper.GenerateJwtToken("userNotSync");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _client.DefaultRequestHeaders.Add("Tenant-Id", Guid.NewGuid().ToString());
        var response = await _client.GetAsync(Route);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "Should fail on any route when the Tenant ID in the header is not a valid UUID")]
    public async Task GivenAnyRoute_WhenTenantIdIsInvalid_ThenFailWithForbidden()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = new User()
            {
                UserName = "username1",
                Email = "username1@domain.com"
            };

            await userManager.CreateAsync(user);

            await userManager.AddLoginAsync(
                user,
                new UserLoginInfo(
                    AuthenticationDefaults.AuthenticationScheme,
                    user.UserName,
                    AuthenticationDefaults.DisplayName
                ));

            var token = JwtHelper.GenerateJwtToken(user.UserName);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _client.DefaultRequestHeaders.Add("Tenant-Id", "invalid-tenant");
            var response = await _client.GetAsync(Route);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }

    [Fact(DisplayName = "Should fail on any route when the tenant is unavailable for the user")]
    public async Task GivenAnyRoute_WhenTenantIsUnavailableForUser_ThenFailWithForbidden()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = new User()
            {
                UserName = "username2",
                Email = "username2@domain.com"
            };

            await userManager.CreateAsync(user);

            await userManager.AddLoginAsync(
                user,
                new UserLoginInfo(
                    AuthenticationDefaults.AuthenticationScheme,
                    user.UserName,
                    AuthenticationDefaults.DisplayName
                ));

            var token = JwtHelper.GenerateJwtToken(user.UserName);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _client.DefaultRequestHeaders.Add("Tenant-Id", Guid.NewGuid().ToString());
            var response = await _client.GetAsync(Route);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
