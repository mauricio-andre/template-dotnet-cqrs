using MediatR;

namespace CqrsProject.Core.Identity.Events;

public record CreateUserTenantEvent(
    Guid UserId,
    Guid TenantId
) : INotification;
