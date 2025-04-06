using CqrsProject.Swagger.Attributes;

namespace CqrsProject.App.RestServer.V1.Users.Dtos;

[SwaggerSchemaIdFilter("Users.SearchUserRoleResponseDto")]
public record SearchUserRoleResponseDto(
    Guid Id,
    string RoleName
);
