using MediatR;

namespace CqrsProject.Core.Identity.Events;

public record CreateRoleClaimEvent(
    Guid RoleId,
    string ClaimType,
    string ClaimValue
) : INotification;
