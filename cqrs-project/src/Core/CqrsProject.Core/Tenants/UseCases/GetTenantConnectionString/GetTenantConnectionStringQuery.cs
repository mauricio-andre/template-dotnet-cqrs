using CqrsProject.Core.Tenants.Responses;
using MediatR;

namespace CqrsProject.Core.Tenants.UseCases.GetTenantConnectionString;

public record GetTenantConnectionStringQuery(
    Guid TenantId,
    Guid Id
) : IRequest<TenantConnectionStringResponse>;
