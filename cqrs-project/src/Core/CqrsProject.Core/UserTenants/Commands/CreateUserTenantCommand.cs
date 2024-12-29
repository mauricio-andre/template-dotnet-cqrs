using MediatR;

namespace CqrsProject.Core.UserTenants.Commands;

public record CreateUserTenantCommand(
    Guid UserId,
    Guid TenantId
) : IRequest;
