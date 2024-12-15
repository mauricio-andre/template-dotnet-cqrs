using MediatR;

namespace CqrsProject.Core.Events;

public record CreateTenantConnectionStringEvent(
    Guid TenantId,
    string ConnectionName
) : INotification;
