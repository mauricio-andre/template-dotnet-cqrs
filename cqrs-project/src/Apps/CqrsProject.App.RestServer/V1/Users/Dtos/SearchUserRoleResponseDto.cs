using CqrsProject.App.RestServer.Filters;

namespace CqrsProject.App.RestServer.V1.Users.Dtos;

[SwaggerSchemaIdFilter("Users.SearchUserRoleResponseDto")]
public record SearchUserRoleResponseDto(
    Guid Id,
    string RoleName
);
