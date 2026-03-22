namespace CqrsProject.App.RestServer.Endpoints.V1.Users.Dtos;

public record UpdateUserRequestDto(
    string UserName,
    string Email,
    string? PhoneNumber
);
