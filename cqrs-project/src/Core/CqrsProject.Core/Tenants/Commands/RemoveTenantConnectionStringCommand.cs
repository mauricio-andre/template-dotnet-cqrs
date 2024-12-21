using MediatR;

namespace CqrsProject.Core.Tenants.Commands;

public record RemoveTenantConnectionStringCommand(
    Guid Id,
    Guid TenantId
) : IRequest;
