using MediatR;

namespace CqrsProject.Core.Events;

public record UpdateTenantEvent(
    Guid Id,
    string Name
) : INotification;
