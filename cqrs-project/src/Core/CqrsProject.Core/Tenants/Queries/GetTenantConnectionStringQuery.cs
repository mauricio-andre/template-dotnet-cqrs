using CqrsProject.Core.Tenants.Responses;
using MediatR;

namespace CqrsProject.Core.Tenants.Queries;

public record GetTenantConnectionStringQuery(
    Guid TenantId,
    Guid Id
) : IRequest<TenantConnectionStringResponse>;
