using CqrsProject.Core.Tenants.Responses;
using MediatR;

namespace CqrsProject.Core.Tenants.Queries;

public record GetTenantByKeyQuery(
    Guid Id
) : IRequest<TenantResponse>;
