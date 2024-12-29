using MediatR;

namespace CqrsProject.Core.UserRoles.Events;

public record CreateUserRoleEvent(
    Guid UserId,
    Guid RoleId
) : INotification;
