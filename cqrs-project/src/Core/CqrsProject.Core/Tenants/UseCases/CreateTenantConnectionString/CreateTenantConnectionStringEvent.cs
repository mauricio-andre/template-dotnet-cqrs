using MediatR;

namespace CqrsProject.Core.Tenants.UseCases.CreateTenantConnectionString;

public record CreateTenantConnectionStringEvent(
    Guid TenantId,
    string ConnectionName
) : INotification;
