namespace CqrsProject.Core.Tenants.Responses;

public record TenantConnectionStringResponse(
    Guid Id,
    Guid TenantId,
    string ConnectionName,
    string KeyName
);
