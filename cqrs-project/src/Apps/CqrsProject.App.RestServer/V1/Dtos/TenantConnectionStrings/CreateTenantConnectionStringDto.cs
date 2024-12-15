namespace CqrsProject.App.RestServer.V1.Dtos;

public record CreateTenantConnectionStringDto(
    string ConnectionName,
    string KeyName
);
