using System.Net.Http.Headers;
using System.Net.Http.Json;
using CqrsProject.Auth0.Dtos;
using CqrsProject.Auth0.Options;
using CqrsProject.Common.Exceptions;
using CqrsProject.Common.Localization;
using CqrsProject.Common.Providers.OAuth.Dtos;
using CqrsProject.Common.Providers.OAuth.Interfaces;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace CqrsProject.Auth0.Services;

public class Auth0Service : IOAuthService
{
    private readonly Auth0Options _auth0Options;
    private readonly IStringLocalizer<CqrsProjectResource> _stringLocalizer;

    public Auth0Service(
        IOptions<Auth0Options> auth0Options,
        IStringLocalizer<CqrsProjectResource> stringLocalizer)
    {
        _auth0Options = auth0Options.Value;
        _stringLocalizer = stringLocalizer;
    }

    public async Task<OAuthUserInfoDto> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync(_auth0Options.UrlUserInfo, cancellationToken);
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<Auth0UserInfoResponse>();

            if (data == null)
                throw new EmptyApiResponseException(_stringLocalizer);

            return new OAuthUserInfoDto(
                Email: data.Email,
                PreferredUserName: data.Email,
                EmailVerified: data.EmailVerified
            );
        }
    }
}
