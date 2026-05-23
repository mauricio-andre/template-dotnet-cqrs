namespace CqrsProject.App.RestServer.Endpoints.V1.Roles.Dtos;

public record SearchUserRoleResponseDto(
    Guid Id,
    string UserName
);
