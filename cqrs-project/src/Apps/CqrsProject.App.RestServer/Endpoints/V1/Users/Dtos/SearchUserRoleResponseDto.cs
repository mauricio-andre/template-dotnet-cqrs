namespace CqrsProject.App.RestServer.Endpoints.V1.Users.Dtos;

public record SearchUserRoleResponseDto(
    Guid Id,
    string RoleName
);
