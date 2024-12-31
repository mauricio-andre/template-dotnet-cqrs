using CqrsProject.App.RestServer.Filters;

namespace CqrsProject.App.RestServer.V1.Tenants.Dtos;

[SwaggerSchemaIdFilter("Tenants.SearchUserTenantResponseDto")]
public record SearchUserTenantResponseDto(
    Guid Id,
    string UserName
);
