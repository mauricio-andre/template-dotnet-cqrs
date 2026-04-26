using MediatR;

namespace CqrsProject.Core.Identity.UseCases.CreateRoleClaim;

public record CreateRoleClaimEvent(
    Guid RoleId,
    string ClaimType,
    string ClaimValue
) : INotification;
