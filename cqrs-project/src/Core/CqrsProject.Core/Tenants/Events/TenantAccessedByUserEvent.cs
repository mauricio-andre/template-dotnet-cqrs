using MediatR;

namespace CqrsProject.Core.Events;

public record TenantAccessedByUserEvent(
    Guid UserId,
    Guid TenantId
) : INotification;
