namespace CqrsProject.Common.Providers.OAuth;

public record OAuthUserInfoDto(
    string Email,
    string PreferredUserName,
    bool EmailVerified
);
