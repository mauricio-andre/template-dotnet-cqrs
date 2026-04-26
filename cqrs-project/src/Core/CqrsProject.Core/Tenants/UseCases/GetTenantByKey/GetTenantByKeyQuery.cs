using CqrsProject.Core.Tenants.Responses;
using MediatR;

namespace CqrsProject.Core.Tenants.UseCases.GetTenantByKey;

public record GetTenantByKeyQuery(
    Guid Id
) : IRequest<TenantResponse>;
