using MediatR;

namespace CqrsProject.Core.UserTenants.UseCases.CreateUserTenant;

public record CreateUserTenantEvent(
    Guid UserId,
    Guid TenantId
) : INotification;
