using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CqrsProject.Commons.Test.Helpers;
using CqrsProject.Commons.Test.Services;
using CqrsProject.Core.Data;
using CqrsProject.Core.Tenants.Entities;
using CqrsProject.Core.UserTenants.Entities;
using CqrsProject.Core.UserTenants.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.App.RestServerTest.Endpoints.V1.Me;

public class MeTenantsTest
    : IClassFixture<RestServerWebApplicationFactory>,
    IClassFixture<UserManageTestService>
{
    private readonly HttpClient _client;
    private readonly RestServerWebApplicationFactory _factory;
    private readonly UserManageTestService _userManageTestService;
    private const string Route = "/v1/me/tenants";

    public MeTenantsTest(RestServerWebApplicationFactory factory, UserManageTestService userManageTestService)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _userManageTestService = userManageTestService;
    }

    [Fact(DisplayName = "Should fail with unauthorized code when the token is missing")]
    public async Task GivenMeTenants_WhenTokenIsMissing_ThenFailWithUnauthorized()
    {
        var response = await _client.GetAsync(Route);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Should fail when the local user is not registered")]
    public async Task GivenMeTenants_WhenUserNotRegistered_ThenReturnForbidden()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var token = JwtHelper.GenerateJwtToken("userNotSync");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync(Route);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }

    [Fact(DisplayName = "Should return a partial result when Take is less than the total available items")]
    public async Task GivenMeTenants_WhenTakeIsLessThanTotal_ThenReturnPartialResult()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var user = await _userManageTestService.CreateMasterAdminUserAsync(scope);

            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AdministrationDbContext>>();
            var context = dbContextFactory.CreateDbContext();

            await context.Tenants.AddRangeAsync(new List<Tenant>()
            {
                new Tenant()
                {
                    Name = "ReturnPartialContent1",
                    UserTenantList = new List<UserTenant>()
                    {
                        new UserTenant()
                        {
                            UserId = user.Id
                        }
                    }
                },
                new Tenant()
                {
                    Name = "ReturnPartialContent2",
                    UserTenantList = new List<UserTenant>()
                    {
                        new UserTenant()
                        {
                            UserId = user.Id
                        }
                    }
                }
            });

            await context.SaveChangesAsync();

            var token = JwtHelper.GenerateJwtToken(user.Email!);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var stringQuery = QueryString.Empty
                .Add("take", "1")
                .Add("sortBy", "Tenant.Name");

            var response = await _client.GetAsync($"{Route}{stringQuery}");

            Assert.Equal(HttpStatusCode.PartialContent, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<IList<MeTenantResponse>>();
            response.Content.Headers.TryGetValues("Content-Range", out var ContentRange);

            Assert.NotNull(content);
            Assert.NotNull(ContentRange);
            Assert.Equal("items 0-1/2", ContentRange.Single());
            Assert.Collection(content, item => Assert.Equal("ReturnPartialContent1", item.TenantName));
        }
    }

    [Fact(DisplayName = "Should return the full list of tenants the user has access to")]
    public async Task GivenMeTenants_WhenUserHasTenants_ThenReturnFullList()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var user = await _userManageTestService.CreateMasterAdminUserAsync(scope);

            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AdministrationDbContext>>();
            var context = dbContextFactory.CreateDbContext();

            await context.Tenants.AddRangeAsync(new List<Tenant>()
            {
                new Tenant()
                {
                    Name = "ReturnAllList1",
                    UserTenantList = new List<UserTenant>()
                    {
                        new UserTenant()
                        {
                            UserId = user.Id
                        }
                    }
                },
                new Tenant()
                {
                    Name = "ReturnAllList2",
                    UserTenantList = new List<UserTenant>()
                    {
                        new UserTenant()
                        {
                            UserId = user.Id
                        }
                    }
                },
                new Tenant()
                {
                    Name = "ReturnAllList3"
                }
            });

            await context.SaveChangesAsync();

            var token = JwtHelper.GenerateJwtToken(user.Email!);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var stringQuery = QueryString.Empty
                .Add("sortBy", "Tenant.Name");

            var response = await _client.GetAsync($"{Route}{stringQuery}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<IList<MeTenantResponse>>();
            response.Content.Headers.TryGetValues("Content-Range", out var ContentRange);

            Assert.NotNull(content);
            Assert.NotNull(ContentRange);
            Assert.Equal("items 0-2/2", ContentRange.Single());
            Assert.Collection(
                content,
                item => Assert.Equal("ReturnAllList1", item.TenantName),
                item => Assert.Equal("ReturnAllList2", item.TenantName));
        }
    }
}
