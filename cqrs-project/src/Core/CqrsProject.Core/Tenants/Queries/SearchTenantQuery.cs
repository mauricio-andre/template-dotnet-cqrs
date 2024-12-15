using CqrsProject.Common.Queries;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Responses;
using MediatR;

namespace CqrsProject.Core.Queries;

public record SearchTenantQuery(
    string? Name,
    int? Take,
    int? Skip,
    string? SortBy,
    bool? IsDeleted = false
) : IPageableQuery, ISortableQuery, IRequest<CollectionResponse<TenantResponse>>;
