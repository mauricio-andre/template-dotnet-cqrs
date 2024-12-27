using MediatR;

namespace CqrsProject.Core.Identity.Commands;

public record RemoveUserTenantCommand(
    Guid UserId,
    Guid TenantId
) : IRequest;
