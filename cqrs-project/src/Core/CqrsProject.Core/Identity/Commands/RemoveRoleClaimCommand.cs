using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record RemoveRoleClaimCommand(
    Guid RoleId,
    string ClaimType,
    string ClaimValue
) : IRequest;
