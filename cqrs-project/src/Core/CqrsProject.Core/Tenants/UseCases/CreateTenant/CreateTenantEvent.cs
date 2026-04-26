using MediatR;

namespace CqrsProject.Core.Tenants.UseCases.CreateTenant;

public record CreateTenantEvent(
    string Name
) : INotification;
