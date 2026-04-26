using MediatR;

namespace CqrsProject.Core.Identity.UseCases.RemoveRoleClaim;

public record RemoveRoleClaimCommand(
    Guid RoleId,
    string ClaimType,
    string ClaimValue
) : IRequest;
