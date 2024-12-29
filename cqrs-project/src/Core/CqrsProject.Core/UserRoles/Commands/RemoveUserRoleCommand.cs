using MediatR;

namespace CqrsProject.Core.UserRoles.Commands;

public record RemoveUserRoleCommand(
    Guid UserId,
    Guid RoleId
) : IRequest;
