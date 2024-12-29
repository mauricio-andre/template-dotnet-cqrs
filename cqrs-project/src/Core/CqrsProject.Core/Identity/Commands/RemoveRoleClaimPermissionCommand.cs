using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record RemoveRoleClaimPermissionCommand(
    Guid RoleId,
    string Name
) : IRequest;
