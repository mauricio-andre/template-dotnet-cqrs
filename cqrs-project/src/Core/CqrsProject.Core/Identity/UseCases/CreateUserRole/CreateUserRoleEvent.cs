using MediatR;

namespace CqrsProject.Core.Identity.UseCases.CreateUserRole;

public record CreateUserRoleEvent(
    Guid UserId,
    Guid RoleId
) : INotification;
