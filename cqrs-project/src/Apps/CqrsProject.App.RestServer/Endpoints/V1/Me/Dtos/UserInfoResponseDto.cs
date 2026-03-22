namespace CqrsProject.App.RestServer.Endpoints.V1.Me.Dtos;

public record UserInfoResponseDto(
    string? Sub,
    string? Name,
    string? Email,
    Dictionary<string, IEnumerable<string>>? Claims
);
