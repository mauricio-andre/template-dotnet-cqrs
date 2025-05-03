using CqrsProject.Swagger.Attributes;

namespace CqrsProject.App.RestServer.Endpoints.V1.Roles.Dtos;

[SwaggerSchemaIdFilter("Roles.SearchUserRoleResponseDto")]
public record SearchUserRoleResponseDto(
    Guid Id,
    string UserName
);
