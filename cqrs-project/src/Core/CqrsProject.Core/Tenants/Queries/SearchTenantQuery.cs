using CqrsProject.Common.Queries;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Tenants.Responses;
using MediatR;

namespace CqrsProject.Core.Tenants.Queries;

public record SearchTenantQuery(
    string? Name,
    int? Take,
    int? Skip,
    string? SortBy,
    bool? IsDeleted = false
) : IPageableQuery, ISortableQuery, IRequest<CollectionResponse<TenantResponse>>;
