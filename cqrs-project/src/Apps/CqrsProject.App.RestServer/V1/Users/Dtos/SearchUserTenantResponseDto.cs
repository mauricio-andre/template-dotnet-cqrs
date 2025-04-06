using CqrsProject.Swagger.Attributes;

namespace CqrsProject.App.RestServer.V1.Users.Dtos;

[SwaggerSchemaIdFilter("Users.SearchUserTenantResponseDto")]
public record SearchUserTenantResponseDto(
    Guid Id,
    string TenantName
);
