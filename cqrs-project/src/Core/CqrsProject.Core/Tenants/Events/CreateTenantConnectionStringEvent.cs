using MediatR;

namespace CqrsProject.Core.Tenants.Events;

public record CreateTenantConnectionStringEvent(
    Guid TenantId,
    string ConnectionName
) : INotification;
