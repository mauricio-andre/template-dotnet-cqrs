using MediatR;

namespace CqrsProject.Core.UserTenants.UseCases.CreateUserTenant;

public record CreateUserTenantCommand(
    Guid UserId,
    Guid TenantId
) : IRequest;
