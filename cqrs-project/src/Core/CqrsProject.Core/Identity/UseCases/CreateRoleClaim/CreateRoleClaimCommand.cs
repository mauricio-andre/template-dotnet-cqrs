using MediatR;

namespace CqrsProject.Core.Identity.UseCases.CreateRoleClaim;

public record CreateRoleClaimCommand(
    Guid RoleId,
    string ClaimType,
    string ClaimValue
) : IRequest;
