namespace CqrsProject.App.RestServer.Endpoints.V1.Tenants.Dtos;

public record CreateTenantConnectionStringRequestDto(
    string ConnectionName,
    string KeyName
);
