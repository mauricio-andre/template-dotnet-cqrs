using MediatR;

namespace CqrsProject.Core.UserTenants.Events;

public record CreateUserTenantEvent(
    Guid UserId,
    Guid TenantId
) : INotification;
