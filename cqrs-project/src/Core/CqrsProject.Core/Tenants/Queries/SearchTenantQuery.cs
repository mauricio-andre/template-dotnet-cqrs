using CqrsProject.Common.Queries;
using CqrsProject.Common.Responses;
using CqrsProject.Core.Responses;
using MediatR;

namespace CqrsProject.Core.Queries;

public record SearchTenantQuery(
    string? Name,
    bool? IsDeleted,
    int? Take,
    int? Skip,
    string? SortBy
) : IPageableQuery, ISortableQuery, IRequest<CollectionResponse<TenantResponse>>;
