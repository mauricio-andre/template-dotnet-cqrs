namespace CqrsProject.App.RestServer.V1.Dtos;

public record CreateTenantConnectionStringRequestDto(
    string ConnectionName,
    string KeyName
);
