using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record RemoveUserRoleCommand(
    Guid UserId,
    Guid RoleId
) : IRequest;
