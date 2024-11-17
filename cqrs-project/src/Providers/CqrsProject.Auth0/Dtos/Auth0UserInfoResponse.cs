using System.Text.Json.Serialization;

namespace CqrsProject.Auth0.Dtos;

public record Auth0UserInfoResponse
{
    public string Sub { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; init; }
}
