using CqrsProject.Swagger.Attributes;

namespace CqrsProject.App.RestServer.Endpoints.V1.Tenants.Dtos;

[SwaggerSchemaIdFilter("Tenants.SearchUserTenantResponseDto")]
public record SearchUserTenantResponseDto(
    Guid Id,
    string UserName
);
