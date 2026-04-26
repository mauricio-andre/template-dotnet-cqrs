using MediatR;

namespace CqrsProject.Core.Tenants.UseCases.RemoveTenantConnectionString;

public record RemoveTenantConnectionStringCommand(
    Guid Id,
    Guid TenantId
) : IRequest;
