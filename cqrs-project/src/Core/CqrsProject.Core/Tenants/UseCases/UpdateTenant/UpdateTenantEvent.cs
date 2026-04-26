using MediatR;

namespace CqrsProject.Core.Tenants.UseCases.UpdateTenant;

public record UpdateTenantEvent(
    Guid Id,
    string Name
) : INotification;
