namespace CqrsProject.App.RestServer.V1.Users.Dtos;

public record UpdateUserRequestDto(
    string UserName,
    string Email,
    string? PhoneNumber
);
