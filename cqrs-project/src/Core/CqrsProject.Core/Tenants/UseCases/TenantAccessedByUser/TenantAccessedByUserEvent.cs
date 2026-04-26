using MediatR;

namespace CqrsProject.Core.Tenants.UseCases.TenantAccessedByUser;

public record TenantAccessedByUserEvent(
    Guid UserId,
    Guid TenantId
) : INotification;
