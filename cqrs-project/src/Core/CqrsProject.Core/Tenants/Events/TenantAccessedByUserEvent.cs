using MediatR;

namespace CqrsProject.Core.Tenants.Events;

public record TenantAccessedByUserEvent(
    Guid UserId,
    Guid TenantId
) : INotification;
