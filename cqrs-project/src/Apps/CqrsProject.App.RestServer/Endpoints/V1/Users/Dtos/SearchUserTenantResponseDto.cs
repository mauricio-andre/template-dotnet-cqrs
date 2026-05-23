namespace CqrsProject.App.RestServer.Endpoints.V1.Users.Dtos;

public record SearchUserTenantResponseDto(
    Guid Id,
    string TenantName
);
