namespace CqrsProject.Core.Identity.UseCases.SearchUserRole;

public record SearchUserRoleResponse(
    Guid UserId,
    Guid RoleId,
    string UserName,
    string RoleName
);
