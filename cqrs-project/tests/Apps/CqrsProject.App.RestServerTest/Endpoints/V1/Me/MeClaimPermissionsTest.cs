using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CqrsProject.Common.Consts;
using CqrsProject.Commons.Test.Helpers;
using CqrsProject.Commons.Test.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.App.RestServerTest.Endpoints.V1.Me;

public class MeClaimPermissionsTest
    : IClassFixture<RestServerWebApplicationFactory>,
    IClassFixture<UserManageTestService>
{
    private readonly HttpClient _client;
    private readonly RestServerWebApplicationFactory _factory;
    private readonly UserManageTestService _userManageTestService;
    private const string Route = $"/v1/me/claims/{AuthorizationPermissionClaims.ClaimType}";

    public MeClaimPermissionsTest(RestServerWebApplicationFactory factory, UserManageTestService userManageTestService)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _userManageTestService = userManageTestService;
    }

    [Fact(DisplayName = "Should fail with unauthorized code when the token is missing")]
    public async Task GivenMePermissions_WhenTokenIsMissing_ThenFailWithUnauthorized()
    {
        var response = await _client.GetAsync(Route);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Should success when the local user is not registered with empty return")]
    public async Task GivenMePermissions_WhenUserNotRegistered_ThenReturnEmpty()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var token = JwtHelper.GenerateJwtToken("userNotSync");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync(Route);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<IList<string>>();

            Assert.Equal(new List<string>(), content);
        }
    }

    [Theory(DisplayName = "Should return the list of 'permission' claims associated with the user")]
    [MemberData(nameof(GetTestData))]
    public async Task WhenUserHasPermissions_ShouldReturnListOfPermissions(
        List<string> input,
        List<string> expected)
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var user = await _userManageTestService.CreateUserAsync(scope, input);

            var token = JwtHelper.GenerateJwtToken(user.Email!);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync(Route);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<IList<string>>();

            Assert.NotNull(content);
            Assert.Equal(expected.Order(), content.Order());
        }
    }

    public static IEnumerable<object[]> GetTestData()
    {
        yield return new object[] {
            new List<string> { },
            new List<string> { }
        };

        yield return new object[] {
            new List<string> { AuthorizationPermissionClaims.ManageExamples },
            new List<string> { AuthorizationPermissionClaims.ManageExamples }
        };

        yield return new object[] {
            new List<string> { AuthorizationPermissionClaims.ManageExamples, AuthorizationPermissionClaims.ManageAdministration },
            new List<string> { AuthorizationPermissionClaims.ManageExamples, AuthorizationPermissionClaims.ManageAdministration }
        };

        yield return new object[] {
            new List<string>
            {
                AuthorizationPermissionClaims.ManageExamples,
                AuthorizationPermissionClaims.ManageAdministration,
                AuthorizationPermissionClaims.ManageSelf
            },
            new List<string>
            {
                AuthorizationPermissionClaims.ManageExamples,
                AuthorizationPermissionClaims.ManageAdministration,
                AuthorizationPermissionClaims.ManageSelf
            },
        };
    }
}
