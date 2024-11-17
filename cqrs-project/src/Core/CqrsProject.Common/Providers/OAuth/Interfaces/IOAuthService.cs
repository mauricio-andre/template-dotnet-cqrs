namespace CqrsProject.Common.Providers.OAuth;

public interface IOAuthService
{
    Task<OAuthUserInfoDto> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken = default);
}
