using CqrsProject.Common.Providers.OAuth.Dtos;

namespace CqrsProject.Common.Providers.OAuth.Interfaces;

public interface IOAuthService
{
    Task<OAuthUserInfoDto> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken = default);
}
