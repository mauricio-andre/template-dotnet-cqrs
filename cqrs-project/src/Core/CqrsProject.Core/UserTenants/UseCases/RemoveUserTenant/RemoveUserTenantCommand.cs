using MediatR;

namespace CqrsProject.Core.UserTenants.UseCases.RemoveUserTenant;

public record RemoveUserTenantCommand(
    Guid UserId,
    Guid TenantId
) : IRequest;
