using MediatR;

namespace CqrsProject.Core.UserRoles.Commands;

public record CreateUserRoleCommand(
    Guid UserId,
    Guid RoleId
) : IRequest;
