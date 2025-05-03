using CqrsProject.Swagger.Attributes;

namespace CqrsProject.App.RestServer.Endpoints.V1.Users.Dtos;

[SwaggerSchemaIdFilter("Users.SearchUserRoleResponseDto")]
public record SearchUserRoleResponseDto(
    Guid Id,
    string RoleName
);
