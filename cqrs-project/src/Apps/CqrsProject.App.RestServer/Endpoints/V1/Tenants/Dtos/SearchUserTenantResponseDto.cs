namespace CqrsProject.App.RestServer.Endpoints.V1.Tenants.Dtos;

public record SearchUserTenantResponseDto(
    Guid Id,
    string UserName
);
