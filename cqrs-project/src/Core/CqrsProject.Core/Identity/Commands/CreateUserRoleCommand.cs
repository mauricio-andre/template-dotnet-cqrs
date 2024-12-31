using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record CreateUserRoleCommand(
    Guid UserId,
    Guid RoleId
) : IRequest;
