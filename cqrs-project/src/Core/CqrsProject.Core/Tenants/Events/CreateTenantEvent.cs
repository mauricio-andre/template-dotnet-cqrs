using MediatR;

namespace CqrsProject.Core.Events;

public record CreateTenantEvent(
    string Name
) : INotification;
