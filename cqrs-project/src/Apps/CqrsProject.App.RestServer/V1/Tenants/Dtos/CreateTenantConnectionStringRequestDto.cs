namespace CqrsProject.App.RestServer.V1.Tenants.Dtos;

public record CreateTenantConnectionStringRequestDto(
    string ConnectionName,
    string KeyName
);
