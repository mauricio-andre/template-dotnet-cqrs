using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record CreateRoleClaimCommand(
    Guid RoleId,
    string ClaimType,
    string ClaimValue
) : IRequest;
