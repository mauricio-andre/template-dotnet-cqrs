namespace CqrsProject.Core.UserRoles.Responses;

public record UserRoleResponse(
    Guid UserId,
    Guid RoleId,
    string UserName,
    string RoleName
);
