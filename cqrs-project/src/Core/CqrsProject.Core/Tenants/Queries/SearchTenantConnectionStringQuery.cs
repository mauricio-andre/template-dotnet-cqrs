using CqrsProject.Common.Queries;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Tenants.Responses;
using MediatR;

namespace CqrsProject.Core.Tenants.Queries;

public record SearchTenantConnectionStringQuery(
    Guid TenantId,
    string? ConnectionName,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery, IRequest<CollectionResponse<TenantConnectionStringResponse>>;
