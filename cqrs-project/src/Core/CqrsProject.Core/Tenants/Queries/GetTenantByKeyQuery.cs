using CqrsProject.Core.Responses;
using MediatR;

namespace CqrsProject.Core.Queries;

public record GetTenantByKeyQuery(
    Guid Id
) : IRequest<TenantResponse>;
