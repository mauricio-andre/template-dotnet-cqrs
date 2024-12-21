namespace CqrsProject.Common.Providers.OAuth.Dtos;

public record OAuthUserInfoDto(
    string Email,
    string PreferredUserName,
    bool EmailVerified
);
