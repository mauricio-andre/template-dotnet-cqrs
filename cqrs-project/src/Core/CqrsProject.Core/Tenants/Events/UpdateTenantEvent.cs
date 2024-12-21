using MediatR;

namespace CqrsProject.Core.Tenants.Events;

public record UpdateTenantEvent(
    Guid Id,
    string Name
) : INotification;
