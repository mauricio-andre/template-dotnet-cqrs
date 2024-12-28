using MediatR;

namespace CqrsProject.Core.UserTenants.Commands;

public record RemoveUserTenantCommand(
    Guid UserId,
    Guid TenantId
) : IRequest;
