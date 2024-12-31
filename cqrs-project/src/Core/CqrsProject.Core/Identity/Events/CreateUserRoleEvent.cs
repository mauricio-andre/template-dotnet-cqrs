using MediatR;

namespace CqrsProject.Core.Identity.Events;

public record CreateUserRoleEvent(
    Guid UserId,
    Guid RoleId
) : INotification;
