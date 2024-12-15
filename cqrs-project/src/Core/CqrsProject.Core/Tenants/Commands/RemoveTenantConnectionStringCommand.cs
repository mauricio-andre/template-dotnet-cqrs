using MediatR;

namespace CqrsProject.Core.Commands;

public record RemoveTenantConnectionStringCommand(
    Guid Id,
    Guid TenantId
) : IRequest;
