using MediatR;

namespace CqrsProject.Core.Tenants.Events;

public record CreateTenantEvent(
    string Name
) : INotification;
