using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record CreateRoleClaimPermissionCommand(
    Guid RoleId,
    string Name
) : IRequest;
