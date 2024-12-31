namespace CqrsProject.Core.Identity.Responses;

public record UserRoleResponse(
    Guid UserId,
    Guid RoleId,
    string UserName,
    string RoleName
);
